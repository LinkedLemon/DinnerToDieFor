using UnityEngine;
using TMPro;
using System.Collections;

public class Hoverable : MonoBehaviour
{
    [Tooltip("The text to display when hovered over.")]
    public string hoverText = "Hover Text";

    public TMP_FontAsset fontType;

    [Tooltip("The offset from the object's center to display the text.")]
    public Vector3 textOffset = new Vector3(0, 1.5f, 0);

    [Tooltip("The font size of the hover text.")]
    public float fontSize = 12f;

    [Tooltip("The color of the hover text.")]
    public Color textColor = Color.white;
    
    [Tooltip("The time it takes for the text to scale in and out.")]
    public float animationTime = 0.2f;

    private TextMeshPro _textMeshPro;
    private GameObject _textObject;
    private Coroutine _animationCoroutine;

    public void OnHoverEnter()
    {
        if (_textObject == null)
        {
            _textObject = new GameObject("HoverText");
            _textObject.transform.SetParent(transform);
            _textObject.transform.localPosition = textOffset;

            _textMeshPro = _textObject.AddComponent<TextMeshPro>();
            _textMeshPro.font = fontType;
            _textMeshPro.text = hoverText;
            _textMeshPro.fontSize = fontSize;
            _textMeshPro.color = textColor;
            _textMeshPro.alignment = TextAlignmentOptions.Center;
            _textMeshPro.transform.rotation = Camera.main.transform.rotation;


        }
        
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }
        _animationCoroutine = StartCoroutine(AnimateScale(Vector3.one));
    }

    public void OnHoverExit()
    {
        if (_textObject != null)
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
            _animationCoroutine = StartCoroutine(AnimateScale(Vector3.zero));
        }
    }

    private IEnumerator AnimateScale(Vector3 targetScale)
    {
        if (_textObject == null) yield break;
        
        Vector3 startScale = _textObject.transform.localScale;
        float time = 0;

        while (time < animationTime)
        {
            _textObject.transform.localScale = Vector3.Lerp(startScale, targetScale, time / animationTime);
            time += Time.deltaTime;
            yield return null;
        }

        _textObject.transform.localScale = targetScale;

        if (targetScale == Vector3.zero)
        {
            Destroy(_textObject);
            _textObject = null;
            _textMeshPro = null;
        }
    }
}
