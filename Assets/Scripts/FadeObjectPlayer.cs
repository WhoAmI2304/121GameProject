using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FadeObjectPlayer : MonoBehaviour
{
    [SerializeField]
    private LayerMask LayerMask;
    [SerializeField]
    private Transform Target;
    [SerializeField]
    private Camera Camera;
    [SerializeField]
    [Range(0.00f, 1.00f)]
    private float FadeAlpha = 0.33f;
    [SerializeField]
    private bool RetainShadows = true;
    [SerializeField]
    private Vector3 TargetPositionOffset = Vector3.up;
    [SerializeField]
    private float FadeSpeed = 1.0f;

    [Header("Read Only Data")]
    [SerializeField]
    private List<FadingObject> ObjectsBlockingView = new List<FadingObject>();
    private Dictionary<FadingObject, Coroutine> RuningCorroutines = new Dictionary<FadingObject, Coroutine>();

    private RaycastHit[] Hits = new RaycastHit[10];

    private void Start()
    {
        StartCoroutine(CheckForObjects());
    }

    private IEnumerator CheckForObjects()
    {
        while(true)
        {
            int hits = Physics.RaycastNonAlloc(
                Camera.transform.position,
                (Target.transform.position + TargetPositionOffset - Camera.transform.position).normalized,
                Hits,
                Vector3.Distance(Camera.transform.position, Target.transform.position),
                LayerMask
            );

            if (hits > 0)
            {
                for (int i = 0; i < hits; i++)
                {
                    FadingObject fadingObject = GetFadingObjectFromHit(Hits[i]);

                    if (fadingObject != null && !ObjectsBlockingView.Contains(fadingObject))
                    {
                        if (RuningCorroutines.ContainsKey(fadingObject))
                        {
                            if(RuningCorroutines[fadingObject] != null)
                            {
                                StopCoroutine(RuningCorroutines[fadingObject]);
                            }

                            RuningCorroutines.Remove(fadingObject);
                        }

                        RuningCorroutines.Add(fadingObject, StartCoroutine(FadeObjectOut(fadingObject)));
                        ObjectsBlockingView.Add(fadingObject);
                    }

                }
            }

            FadeObjectsNoLongerBeingHit();

            ClearHits();

            yield return null;
        }
    }

    private IEnumerator FadeObjectOut(FadingObject FadingObject)
    {
        foreach(Material material in FadingObject.Materials)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.SetInt("_Surface", 1);

            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            material.SetShaderPassEnabled("DepthOnly", false);
            material.SetShaderPassEnabled("SHADOWCASTER", RetainShadows);

            material.SetOverrideTag("RenderType", "Transparent");

            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        float time = 0.0f;

        while (FadingObject.Materials[0].color.a > FadeAlpha)
        {
            foreach (Material material in FadingObject.Materials)
            {
                if (material.HasProperty("_Color"))
                {
                    material.color = new Color(
                        material.color.r,
                        material.color.g,
                        material.color.b,
                        math.lerp(FadingObject.InitialAlpha, FadeAlpha, time * FadeSpeed)
                    );

                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        if (RuningCorroutines.ContainsKey(FadingObject))
        {
            StopCoroutine(RuningCorroutines[FadingObject]);
            RuningCorroutines.Remove(FadingObject);
        }
    }

    private IEnumerator FadeObjectIn(FadingObject FadingObject)
    {
        float time = 0.0f;

        while (FadingObject.Materials[0].color.a < FadingObject.InitialAlpha)
        {
            foreach (Material material in FadingObject.Materials)
            {
                if (material.HasProperty("_Color"))
                {
                    material.color = new Color(
                        material.color.r,
                        material.color.g,
                        material.color.b,
                        math.lerp(FadeAlpha, FadingObject.InitialAlpha, time * FadeSpeed)
                    );

                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        foreach(Material material in FadingObject.Materials)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.SetInt("_Surface", 0);

            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

            material.SetShaderPassEnabled("DepthOnly", true);
            material.SetShaderPassEnabled("SHADOWCASTER", true);

            material.SetOverrideTag("RenderType", "Opaque");

            material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        

        if (RuningCorroutines.ContainsKey(FadingObject))
        {
            StopCoroutine(RuningCorroutines[FadingObject]);
            RuningCorroutines.Remove(FadingObject);
        }
    }

    private void FadeObjectsNoLongerBeingHit()
    {
        List<FadingObject> objectsToRemove = new List<FadingObject>(ObjectsBlockingView.Count);

        foreach(FadingObject fadingObject in ObjectsBlockingView)
        {
            bool isBeingHit = false;

            for (int i = 0; i < Hits.Length; i++)
            {
                FadingObject hitFadingObject = GetFadingObjectFromHit(Hits[i]);
                if(hitFadingObject != null && fadingObject == hitFadingObject)
                {
                    isBeingHit = true;  
                    break;
                }
            }

            if (!isBeingHit)
            {
                if (RuningCorroutines.ContainsKey(fadingObject))
                {
                    if (RuningCorroutines[fadingObject] != null)
                    {
                        StopCoroutine(RuningCorroutines[fadingObject]);
                    }

                    RuningCorroutines.Remove(fadingObject);
                }

                RuningCorroutines.Add(fadingObject, StartCoroutine(FadeObjectIn(fadingObject)));
                objectsToRemove.Add(fadingObject);
            }
        }

        foreach(FadingObject removeObject in objectsToRemove)
        {
            ObjectsBlockingView.Remove(removeObject);
        }
    }

    private void ClearHits()
    {
        System.Array.Clear(Hits, 0, Hits.Length);
    }
    private FadingObject GetFadingObjectFromHit(RaycastHit hit)
    {
        return hit.collider != null ? hit.collider.GetComponent<FadingObject>() : null; 
    }
}
