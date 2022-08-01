using System.Collections.Generic;
using UnityEngine;

/// <summary> A class that stores information about some animation. 
/// For example an equippable item like a Leather Boot, defines the animation inside the Animation Controller
/// with the name defined in a GameObject with the defined name will be the defined AnimationClip.
/// </summary>

// pt-br
/// <summary> Uma classe que guarda informações de alguma animação.
/// Por exemplo um item equipável como uma Bota de Couro, define a animação dentro do Animation Controller
/// com o nome definido em um GameObject com o nome definido será o AnimationClip definido.
/// Contém Animação para o corpo todo
/// </summary>

[System.Serializable]
public class AnimationMap
{
    /*Como funciona um AnimationMap?
        Um SpriteMap define um AnimationClip para uma determinada animação em uma determinada parte do sprite do Alive;
        Por exemplo, um AnimationMap com o nome da animação 'Idle' e com o nome da parte do corpo 'Feet', vai definir o AnimationClip do 'Feet' na animação 'Idle';
        Um Alive não pode ter dois SpriteMaps com mesmo nome de animação e de parte do corpo;
    */

    /// <summary> Nome da parte do corpo que será afetada. Por exemplo: Feet </summary>
    public string gameObjectName = "none";
    /// <summary> As animações para essa parte do corpo. </summary>
    public List<SpriteMap> spriteMapList = new List<SpriteMap>();

    /// <summary> Remove todas as animações desse AnimationMap. </summary>
    public void Clean()
    {
        int index = 0;
        foreach (var item in spriteMapList)
        {
            spriteMapList[index].animation = null;
        }
        index++;
    }

    /// <summary> Retorna uma cópia de valores dessa referência. </summary>
    public AnimationMap GetCopy()
    {
        AnimationMap result = new AnimationMap();
        result = (AnimationMap)this.MemberwiseClone();

        List<SpriteMap> newSriteMapList = new List<SpriteMap>();

        foreach (var item in spriteMapList)
        {
            newSriteMapList.Add(item.GetCopy());
        }

        result.spriteMapList = newSriteMapList;
        return result;
    }

    /// <summary> Retorna verdadeiro se esse os valores forem iguais. </summary>
    public override bool Equals(object obj)
    {
        AnimationMap animationMap = (AnimationMap)obj;

        bool nameIsEqual = gameObjectName == animationMap.gameObjectName;
        bool sameCount = spriteMapList.Count == animationMap.spriteMapList.Count;
        bool sameSpriteMapList = true;

        if (sameCount == true)
        {
            int index = 0;
            foreach (SpriteMap spriteMap in spriteMapList)
            {
                if (!spriteMap.Equals(animationMap.spriteMapList[index]))
                {
                    sameSpriteMapList = false;
                    break;
                }
                index++;
            }
        }
        else { sameSpriteMapList = false; }

        return nameIsEqual && sameCount && sameSpriteMapList;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

/// <summary> Contém uma animação para uma parte do corpo</summary>
[System.Serializable]
public class SpriteMap
{
    /// <summary> O nome da animação desse animation clip. Por exemplo: IdleWeapon </summary>
    public string animationName = "none";
    /// <summary> A nova animação que será aplicada. </summary>
    public AnimationClip animation;

    /// <summary> Cria um novo SpriteMap com os valores dados. </summary> 
    /// <param name="inAnimationName"> Nome da animação no animation controller que será afetado.</param> 
    /// <param name="inAnimation"> A nova animação que será aplicada.</param>
    public SpriteMap(string inAnimationName, AnimationClip inAnimation)
    {
        animationName = inAnimationName;
        animation = inAnimation;
    }

    public SpriteMap GetCopy()
    {
        return (SpriteMap)this.MemberwiseClone();
    }

    public override bool Equals(object obj)
    {
        SpriteMap spriteMap = (SpriteMap)obj;

        return animationName == spriteMap.animationName && animation == spriteMap.animation;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}