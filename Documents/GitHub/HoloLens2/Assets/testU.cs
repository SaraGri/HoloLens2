
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using NumericsConversion;
using Microsoft.MixedReality.SceneUnderstanding;

#if ENABLE_WINMD_SUPPORT
using Microsoft.MixedReality.SceneUnderstanding;
using Windows.Perception.Spatial;
using Windows.Perception.Spatial.Preview;
using UnityEngine.XR.WSA;
#endif

public class testU : MonoBehaviour
{
    public GameObject root1;
    //eine neue Szene erzeugen, die auf dem Mixed Reality-Headset gelegt wird
    public Scene underStandingscene;
    private bool initialised;
   // private float understandingRadius;
    // Start is called before the first frame update
    void Start()
    {
        initialised = false;
        Debug.Log("Hallo Welt");
    }

    // Update is called once per frame
    void Update()
    {
#if ENABLE_WINMD_SUPPORT
        // TODO: Doing all of this every update right now based on what I saw in the doc 
        // here https://docs.microsoft.com/en-us/windows/mixed-reality/scene-understanding-sdk#dealing-with-transforms
        // but that might be overkill.
        // Additionally, wondering to what extent I should be releasing these COM objects
        // as I've been lazy to date.
        // Hence - apply a pinch of salt to this...
        if (this.underStandingscene != null)
        {
            var node = this.underStandingscene.OriginSpatialGraphNodeId;

            var sceneCoordSystem = SpatialGraphInteropPreview.CreateCoordinateSystemForNode(node);

            var unityCoordSystem =
                (SpatialCoordinateSystem)System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(
                    WorldManager.GetNativeISpatialCoordinateSystemPtr());

            var transform = sceneCoordSystem.TryGetTransformTo(unityCoordSystem);

            if (transform.HasValue)
            {
                var sceneToWorldUnity = transform.Value.ToUnity();

                this.root1.transform.SetPositionAndRotation(
                    sceneToWorldUnity.GetColumn(3), sceneToWorldUnity.rotation);
            }
        }
#endif

    }

   
    public void startSpatialUnderstanding()
    {
#if WINDOWS_UWP
        InitialiseAsync();
#endif
    }


    //https://github.com/mtaulty/SceneUnderstanding/blob/master/Assets/Scripts/MyScript.cs
#if ENABLE_WINMD_SUPPORT
    async Task InitialiseAsync()
    {
            if (SceneObserver.IsSupported())
            {
                var access = await SceneObserver.RequestAccessAsync();

                if (access == SceneObserverAccessStatus.Allowed)
                {
                    this.initialised = true;
                } 
            }
    }

     async Task ComputeAsync(SceneObjectKind sceneObjectKind, float radius)
    {
       
        await this.InitialiseAsync();

        if (this.initialised)
        {
            var querySettings = new SceneQuerySettings()
            {
                EnableWorldMesh = false,
                EnableSceneObjectQuads = true,
                EnableSceneObjectMeshes = false,
                EnableOnlyObservedSceneObjects = false
            };
            this.underStandingscene = await SceneObserver.ComputeAsync(querySettings, radius);
        }
    }

#endif

    public async void OnPlatform()
    {
#if ENABLE_WINMD_SUPPORT
        await this.ComputeAsync(SceneObjectKind.Platform, 10.0f);
#endif
    }
   
}
//https://docs.microsoft.com/de-de/windows/mixed-reality/scene-understanding-sdk#sceneobjects
//https://mtaulty.com/2019/10/22/baby-steps-with-the-scene-understanding-sdk/ 