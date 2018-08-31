using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffect : MonoBehaviour {

    public MaskKind maskKind;

    public Shader shader;

    Material _mat;
    protected Material mat
    {
        get
        {
            if (_mat == null)
                _mat = new Material(shader);

            return _mat;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (shader == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        Graphics.Blit(source, destination, mat, (int)maskKind);
    }

    public enum MaskKind
    {
        None = 0,
        Cube = 1,
        Sphere = 2,
        Capsule = 3,
    }
}
