using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class AllergyGridButton : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    [SerializeField] private RawImage iconImage;

    [Header("Icons")]
    [SerializeField] private Texture happyIcon;
    [SerializeField] private Texture neutralIcon;
    [SerializeField] private Texture upsetIcon;
    [SerializeField] private Texture deadIcon;
    [SerializeField] private Texture allergyIcon; 

    private int _currentIndex = 1;
    private bool _isLocked = false;
    private List<Texture> _cycleIcons;

    private void Awake()
    {
        _cycleIcons = new List<Texture> { happyIcon, neutralIcon, upsetIcon };
        // Initialize with the first icon if available
        if (_cycleIcons.Count > 0 && iconImage != null)
        {
            iconImage.texture = _cycleIcons[0];
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isLocked || iconImage == null || _cycleIcons.Count == 0) return;

        _currentIndex = (_currentIndex + 1) % _cycleIcons.Count;
        UpdateIcon();
    }

    public void SetLockedState(bool isDead, bool isAllergy)
    {
        _isLocked = true;
        if (isDead)
        {
            if (deadIcon != null) iconImage.texture = deadIcon;
        }
        else if (isAllergy)
        {
            if (allergyIcon != null) iconImage.texture = allergyIcon;
        }
        else
        {
            // If locked but neither state specified, just unlock? 
            // Or keep locked with current icon? 
            // For safety, let's unlock or assume error. 
            // Based on usage, we'll assume we only call this with true/true.
            _isLocked = false;
            UpdateIcon();
        }
    }
    
    public void Unlock()
    {
        // Only unlock if we were locked.
        // Also we should restore the user's last note (cycle state)
        _isLocked = false;
        UpdateIcon();
    }
    
    public void ResetToDefault()
    {
        _currentIndex = 1; 
        _isLocked = false;
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        if (!_isLocked && _cycleIcons.Count > 0 && iconImage != null)
        {
            iconImage.texture = _cycleIcons[_currentIndex];
        }
    }
}
