using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour {
    public MeshRenderer _meshRenderer;
    public SkinnedMeshRenderer _skinnedMeshRenderer;
    public Material[] _materials;

    int _id = 0;
	// Use this for initialization
	void Start () {
        if (_meshRenderer == null)
            _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (_meshRenderer == null && _skinnedMeshRenderer == null)
            _skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
    }
	
    public void Next()
    {
        SetMaterial(_id + 1);
    }
    public void Previous()
    {
        SetMaterial(_id - 1);
    }
    public void SetMaterial(int id)
    {
        if (_materials == null || _meshRenderer == null || _materials.Length == 0)
            return;

        _id = (id < 0) ? _materials.Length - 1 : (id > _materials.Length - 1) ? 0 : id;

        if(_meshRenderer!=null)
            _meshRenderer.material = _materials[_id];
        else
            _skinnedMeshRenderer.material = _materials[_id];
    }
}
