using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class StencilMaskGenerator : MonoBehaviour
{
    public Shader shader;
    private Material _mat;
    public Material mat
    {
        get
        {
            _mat = new Material(shader);
            return _mat;
        }
    }

    private new Camera camera;

    private CommandBuffer buffer;
    private int cameraRenderingTextureID;

    [SerializeField]
    StencilMaskData[] maskData;

    private void OnEnable()
    {
        camera = GetComponent<Camera>();

        Setup();
        camera.AddCommandBuffer(CameraEvent.AfterForwardAlpha, buffer);
    }

    private void OnDisable()
    {
        camera.RemoveCommandBuffer(CameraEvent.AfterForwardAlpha, buffer);
    }

    private void Setup()
    {
        if (buffer == null)
        {
            buffer = new CommandBuffer { name = "commandBuffer" };

            if (cameraRenderingTextureID == 0) { cameraRenderingTextureID = Shader.PropertyToID("_tempTex"); }

            buffer.GetTemporaryRT(cameraRenderingTextureID, -1, -1, 0);

            buffer.Blit(BuiltinRenderTextureType.CameraTarget, cameraRenderingTextureID);

            for (int i = 0; i < maskData.Length; i++)
            {
                var stencil = maskData[i].targetStencil;
                var maskName = maskData[i].maskName;

                var material = new Material(shader);

                material.SetInt("_Stencil", stencil);
                var outputId = Shader.PropertyToID("_outputTex_" + maskName);
                buffer.GetTemporaryRT(outputId, -1, -1, 0);
                buffer.Blit(cameraRenderingTextureID, BuiltinRenderTextureType.CameraTarget, material);
                buffer.Blit(BuiltinRenderTextureType.CameraTarget, outputId);
                buffer.SetGlobalTexture(maskName, outputId);
            }

            buffer.Blit(cameraRenderingTextureID, BuiltinRenderTextureType.CameraTarget);
        }
    }

    [System.Serializable]
    public class StencilMaskData
    {
        public int targetStencil;
        public string maskName;
    }
}
