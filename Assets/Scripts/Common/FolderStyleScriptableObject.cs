using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Folder Style", menuName ="Game/Folder Style")]
public class FolderStyleScriptableObject : ScriptableObject
{
    [Header("Sprites")]
    public Sprite defaultSprite;
    public Sprite upgradeSprite;
    public Sprite secretSprite;
    public Sprite bossSprite;
    [Tooltip("Optionnal")]
    public Sprite specialSprite;

    [Header("Colors")]
    [ColorUsageAttribute(true, true)]
    public Color defaultSpriteColor;
    [ColorUsageAttribute(true, true)]
    public Color upgradeSpriteColor;
    [ColorUsageAttribute(true, true)]
    public Color secretSpriteColor;
    [ColorUsageAttribute(true, true)]
    public Color specialSpriteColor;
    [ColorUsageAttribute(true, true)]
    public Color bossSpriteColor;
}
