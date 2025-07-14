using System.Collections;
using UnityEngine;

public class ImpactFlash : MonoBehaviour
{
    // �������� ��������� �� ���������� ����� Resources (Resources/Flash.mat)
    [SerializeField] private string flashMaterialName = "Flash";
    private Material flashMaterial;
    private Material originalMaterial; // ��������� ������������ ��������

    public void Flash(SpriteRenderer spriteRenderer, float duration)
    {
        if (flashMaterial == null)
        {
            flashMaterial = Resources.Load<Material>(flashMaterialName);
            if (flashMaterial == null)
            {
                Debug.LogError("Flash material not found in Resources: " + flashMaterialName);
                return;
            }
        }

        // ��������� ������������ �������� ������ ���� ���
        if (originalMaterial == null)
        {
            originalMaterial = spriteRenderer.sharedMaterial;
        }

        StartCoroutine(DoFlash(spriteRenderer, duration));
    }

    private IEnumerator DoFlash(SpriteRenderer spriteRenderer, float duration)
    {
        spriteRenderer.material = flashMaterial;
        var saveColor = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(duration);
        spriteRenderer.color = saveColor;
        spriteRenderer.material = originalMaterial;
    }
}