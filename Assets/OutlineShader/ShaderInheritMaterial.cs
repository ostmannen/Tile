using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderInheritMaterial : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Debug Selection")]
    private bool selected;

    private MeshRenderer mR;

    // Start is called before the first frame update
    void OnEnable()
    {
        mR = GetComponent<MeshRenderer>();
        selected = false;
        mR.materials[1].SetColor("_BaseColour", mR.materials[0].color);

        SetMaterial();
    }
    public void Select()
    {
        selected = !selected;

        SetMaterial();
    }
    private void SetMaterial()
    {
        if (selected)
        {
            mR.materials[1].SetFloat("_selected", 1);
            mR.materials[2].SetFloat("_selected", 1);

            mR.materials[2]
                .SetFloat("_OutlineHeightOffset", mR.materials[2].GetFloat("_Thickness") * -1);
        }
        else
        {
            mR.materials[1].SetFloat("_selected", 0);
           mR.materials[2].SetFloat("_selected", 0);
        }
    }
    private void OnValidate()
    {
        //SetMaterial();
    }
}
