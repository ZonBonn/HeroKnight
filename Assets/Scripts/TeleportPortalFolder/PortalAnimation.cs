using System.Linq;
using UnityEngine;

public class PortalAnimation : MonoBehaviour
{
    public Sprite[] portalSprites;
    private const float EachFramesPortalChange = 0.07f;
    private float m_EachFramesPortalChange;
    private int idxFrames;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        idxFrames = 0;
        m_EachFramesPortalChange = EachFramesPortalChange;
    }

    private void Update()
    {
        m_EachFramesPortalChange -= Time.deltaTime;
        if(m_EachFramesPortalChange <= 0)
        {
            ++idxFrames;
            if(idxFrames == portalSprites.Length)
            {
                idxFrames = 0;
            }
            spriteRenderer.sprite = portalSprites[idxFrames];
            m_EachFramesPortalChange = EachFramesPortalChange; 
        }
    }
}
