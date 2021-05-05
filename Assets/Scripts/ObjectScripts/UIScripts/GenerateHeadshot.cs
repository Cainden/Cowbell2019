using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

/// <summary>
/// Generates a headshot for use with multiple UI.Image objects
/// layered over each other. 
/// </summary>
[RequireComponent(typeof(WorkerToHire), typeof(SpriteLibrary))]
public class GenerateHeadshot : MonoBehaviour
{
    [SerializeField]
    private GameObject m_hat;
    private UnityEngine.UI.Image m_hatImage;

    [SerializeField]
    private GameObject m_head;
    private UnityEngine.UI.Image m_headImage;

    [SerializeField]
    private GameObject m_body;
    private UnityEngine.UI.Image m_bodyImage;

    private SpriteLibrary m_spriteLibrary;
    private bool m_initializedSusscessfully;

    private static readonly string HAT = "hat";
    private static readonly string HEAD = "Head";
    private static readonly string BODY = "Body";

    private void Awake()
    {
        // Cache sprite library
        if (m_spriteLibrary == null)
        {
            m_spriteLibrary = GetComponent<SpriteLibrary>();
        }

        // Make sure prefab is set up correctly
        if (m_hat == null || m_head == null || m_body == null)
        {
            Debug.LogWarning("GenerateHeadshot.Awake : At least one of the headshot GameObjects are null!");
        }
        else
        {
            m_hatImage = m_hat.GetComponent<UnityEngine.UI.Image>();
            m_headImage = m_head.GetComponent<UnityEngine.UI.Image>();
            m_bodyImage = m_body.GetComponent<UnityEngine.UI.Image>();

            if (m_hatImage == null || m_headImage == null || m_bodyImage == null)
            {
                Debug.LogWarning("GenerateHeadshot.Awake : At least one of the headshot UI.Image objects are null!");
            }
            else
            {
                m_initializedSusscessfully = true;
            }
        }

        PerformCleanup(); // HACK       
    }

    /// <summary>
    /// Creates a headshot for use in the UI.
    /// </summary>
    /// <param name="label">
    /// The name of the sprite to use from the
    /// SpriteLibraryAsset attached to the SpriteLibrary attached to this
    /// object.
    /// </param>
    public void CreateHeadshot(CharacterSwaper.CharLabel label)
    {
        if (m_initializedSusscessfully == false)
        {
            return;
        }

        string labelToString = label.ToString();

        SetImage(m_hatImage, HAT, labelToString);
        SetImage(m_headImage, HEAD, labelToString);
        SetImage(m_bodyImage, BODY, labelToString);
    }

    /// <summary>
    /// Sets the image passed in using a category and image label using the
    /// SpriteLibrary attached to this game object.
    /// </summary>
    /// <param name="image">UI.Image object to replace image of</param>
    /// <param name="category">Category used by SpriteLibrary</param>
    /// <param name="imageLabel">Image label used by SpriteLibrary</param>
    private void SetImage(UnityEngine.UI.Image image, string category, string imageLabel)
    {
        Sprite sprite = m_spriteLibrary.GetSprite(category, imageLabel);
        if (sprite != null)
        {
            image.sprite = sprite;
            image.preserveAspect = true;
        }
        else
        {
            Debug.LogWarning(category + " sprite not found for label : " + imageLabel);
        }
    }

    // HACK : This function is a hacky way to handle object offsets, and should
    // go away!!! In the future we should move towards using an anchor/pivot to
    // align sprites.
    private void PerformCleanup()
    {
        // This is a very temporary fix that just makes things look a little better. This
        // is not a final fix. Please don't add to this!!

        m_hat.GetComponent<RectTransform>().sizeDelta = new Vector2(68.0f, 68.0f);
        m_hat.GetComponent<RectTransform>().Translate(new Vector3(-11.9f, 39.1f, 0.0f));

        m_body.GetComponent<RectTransform>().Translate(new Vector3(0.0f, -47.2f, 0.0f));
    }
}
