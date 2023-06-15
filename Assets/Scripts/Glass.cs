using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour
{
    public bool isBroken;
    private MeshRenderer _meshRenderer;
    private BoxCollider boxCollider;

    public GameObject brokenGlassModel;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
    }

    public void BreakGlass()
    {
        _meshRenderer.enabled = false;
        boxCollider.enabled = false;
        brokenGlassModel.SetActive(true);
    }
}
