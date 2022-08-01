using System.Collections.Generic;
using UnityEngine;

public class DynamicSprites : MonoBehaviour
{
    [Header("Sprites")]
    /// <summary> Component that contais the Skin Gameobject, that sprite is inteded to be behind of all others Sprite Components. </summary> en-us
    /// <summary> Componente que contém o Gameobject da skin, esse Gameobject é destinado para estar atrás de todos os outros sprites components. </summary> pt-br
    public GameObject SkinComponent;
    /// <summary> A list of all of SpriteComponents excluding the Skin Component. </summary> en-us
    /// <summary> Uma lista de todos os SpriteComponents, exceto Skin Component. </summary> pt-br
    public List<GameObject> spriteComponentList = new List<GameObject>();
    // 0: Feet
    // 1: Legs
    // 2: Body
    // 3: Arms
    // 4: Head
    // 5: Weapon

    public List<AnimationMap> AnimationMapList = new List<AnimationMap>();
    public List<AnimationMap> SkinAnimationMapList = new List<AnimationMap>();

    public void AddAnimationMap(AnimationMap animationMap)
    {
        int index = 0;
        AnimationMap animation = new AnimationMap();
        foreach (AnimationMap item in AnimationMapList)
        {
            if (item.gameObjectName == animationMap.gameObjectName)
            {
                if (animationMap.spriteMapList.Count > 0)
                {
                    animation = animationMap.GetCopy();
                    RefreshAnimations(GetSpriteComponent(item.gameObjectName));
                }
                else
                {
                    RemoveAnimationMap(animationMap.GetCopy().gameObjectName);
                }

                AnimationMapList[index] = animation;
                return;
            }
        }

        AnimationMapList.Add(animationMap.GetCopy());
        RefreshAllAnimations();
    }

    public bool RemoveAnimationMap(string gameObjectName)
    {
        int index = 0;
        int indexToRemove = -1;
        foreach (var item in AnimationMapList)
        {
            if (item.gameObjectName == gameObjectName)
            {
                indexToRemove = index;
                break;
            }
            index++;
        }

        if (indexToRemove != -1)
        {
            AnimationMapList.RemoveAt(indexToRemove);
            RefreshAllAnimations();
            return true;
        }

        return false;
    }

    protected void RefreshAnimations(GameObject spriteComponent)
    {
        spriteComponent.GetComponent<SpriteComponent>().RefreshAnimations();
    }

    protected void RefreshSkinAnimation()
    {
        SkinComponent.GetComponent<SpriteComponent>().RefreshAnimations(SkinAnimationMapList);
    }

    protected void RefreshAllAnimations()
    {
        foreach (GameObject item in spriteComponentList)
        {
            item.GetComponent<SpriteComponent>().RefreshAnimations();
        }
        RefreshSkinAnimation();
    }

    public GameObject GetSpriteComponent(string name)
    {
        foreach(GameObject item in spriteComponentList)
        {
            if (item.name == name)
            {
                return item;
            }
        }
        return null;
    }
}
