using System.Collections.Generic;
using UnityEngine;

/// <summary> Save the overrides of animatorOverrideController, then apply it. </summary> en-us
/// <summary> Salva os overrides do animatorOverrideController para depois aplicá-los. </summary> pt-br
[System.Serializable]
public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) {}

    public AnimationClip this[string name]
    {
        get 
        { 
            return this.Find(x => x.Key.name.Equals(name)).Value; 
        }
        set
        {
            int index = this.FindIndex(x => x.Key.name.Equals(name));

            if (index != -1)
            {
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
            }
        }
    }
}

/// <summary> Class used on GameObjects that contains "DynamicSprites.cs" code.</summary> en-us
/// <summary> Classe usada nos GameObjects que contenham o código de "DynamicSprites.cs".</summary> pt-br
public class SpriteComponent : MonoBehaviour
{
    public bool isSkinAnimation = false;

    /// <summary> Sprite component. </summary> en-us
    /// <summary> Componente do Sprite. </summary> pt-br
    public SpriteRenderer spriteRendererComponent;
    /// <summary> Main animator. </summary> en-us
    /// <summary> O Animator principal, o qual será derivado. </summary> pt-br
    protected Animator animatorComponent;
    /// <summary> The animator controller that will be applied in that SpriteComponent. </summary> en-us
    /// <summary> O animator controller que será aplicado nesse SpriteComponent, somente nesse. </summary> pt-br
    protected AnimatorOverrideController animatorOverrideController;
    /// <summary> Used for saving and defining animations. </summary> en-us
    /// <summary> Usado para salvar e definir animações. </summary> pt-br
    public AnimationClipOverrides clipOverrides;
    /// <summary> Reference to object's owner. </summary> pt-br
    /// <summary> Referência ao objeto dono. </summary> pt-br
    public DynamicSprites aliveReference;

    void Awake()
    {
        InitializeVariables();

        if (!aliveReference)
        {
            aliveReference = transform.parent.gameObject.GetComponent<Alive>();
        }

        if (isSkinAnimation == false)
        {
            if (!aliveReference.spriteComponentList.Contains(this.gameObject))
                aliveReference.spriteComponentList.Add(this.gameObject); // Se adiciona na lista de sprites do Alive || Adds itself to Alive's sprite list
        }
        else
        {
            aliveReference.SkinComponent = this.gameObject; // Define que é o component de pele do Alive || Defines what is the skin component of Alive
        }

    }

    void Start() 
    {
        // Define as animações pela primeira vez
        // Set animations for the first time
        if (isSkinAnimation == false)
        {
            SetAnimation(aliveReference.AnimationMapList);
        }
        else
        {
            SetAnimation(aliveReference.SkinAnimationMapList);
        }
    }

    void Update()
    {
        animatorComponent.SetFloat("Speed", Mathf.Abs(aliveReference.CurrentSpeedX));
        animatorComponent.SetBool("IsInAir", aliveReference.isInAir);
        animatorComponent.SetFloat("VerticalSpeed", aliveReference.CurrentSpeedY);
    }

    /// <summary> Set character directions based on the give int, 1 is for the right and -1 to the left. </summary> en-use
    /// <summary> Define a direção do personagem baseado na direção dada, 1 para direita, outro número para esquerda. </summary> pt-br
    public void OrientRotationTo(int direction)
    {
        switch(direction)
        {
            case -1:
                spriteRendererComponent.flipX = true;
                break;
            case 1:
                spriteRendererComponent.flipX = false;
                break;   
            default:
                break; 
        }
    }
    
    /// <summary> Refresh animations based on DynamicSprites AnimationSpriteMapList. </summary> en-us
    /// <summary> Recarrega as animações baseado no AnimationSpriteMapList do Alive. </summary> pt-br
    public void RefreshAnimations()
    {
        SetAnimation(aliveReference.AnimationMapList);
    }

    /// <summary> Refresh animations. </summary> en-us
    /// <summary> Recarrega as animações. </summary> pt-br
    public void RefreshAnimations(List<AnimationMap> animationMaps)
    {
        SetAnimation(animationMaps);
    }

    private void InitializeVariables()
    {
        spriteRendererComponent = this.gameObject.GetComponent<SpriteRenderer>(); // Define o componente do sprite
        animatorComponent = this.gameObject.GetComponent<Animator>(); // Define o animator component

        animatorOverrideController = new AnimatorOverrideController(animatorComponent.runtimeAnimatorController); // Cria o Animator Override, para ser usado somente nesse GameObject
        animatorComponent.runtimeAnimatorController = animatorOverrideController; // Define no Animator o animator vigente, que é o que acabou de ser criado

        clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
        animatorOverrideController.GetOverrides(clipOverrides);
    }

    /// <summary> Set the animations bases on a given AnimationMap. </summary> en-us
    /// <summary> Define as animações baseado em um dado AnimationMap. </summary> pt-br
    private void SetAnimation(List<AnimationMap> AnimationMapList)
    {
        RemoveCurrentAnimation(); // Chama essa função para caso não haja nenhum Animation Map para esse SpriteComponent. Pois, caso não haja nenhum Animation Map, a função abaixo não será executado

        bool shouldResetSprite = true;
        foreach (AnimationMap animationMapItem in AnimationMapList) // Procura em todos os Animations Maps
        {
            if (animationMapItem.gameObjectName == this.gameObject.name) // Se corresponder a esse component
            {
                foreach (SpriteMap spriteMapItem in animationMapItem.spriteMapList) // Vai definir todas as animações
                {
                    shouldResetSprite = false;
                    clipOverrides[spriteMapItem.animationName] = spriteMapItem.animation;
                }
            }
        }

        if (shouldResetSprite) spriteRendererComponent.sprite = null;
        animatorOverrideController.ApplyOverrides(clipOverrides);
    }

    /// <summary> Set all animations to 'null'. </summary> en-us
    /// <summary> Define todas as animações para 'null'. </summary> pt-br
    private void RemoveCurrentAnimation()
    {
        // o único jeito de funcionar que achei foi esse
        // Ian do futuro não me pergunte o porquê, só saiba que funciona

        // Cria um novo override para ser o substituto, a quantidade é definida pela quantidade de animações que o controller tiver
        AnimationClipOverrides newAnimationClip = new AnimationClipOverrides(animatorOverrideController.overridesCount);
        animatorOverrideController.GetOverrides(newAnimationClip); // ACHO que define quais são as animações existentes do AnimatorController, no newAnimationClip

        // Por algum motivo, apenas da Clean no clipOverrides e fazer os passos acima não funciona 

        foreach(var item in clipOverrides) // A cada override existente
        {
            newAnimationClip[item.Key.name] = new AnimationClip(); // Define no substituto uma animação nula
        }

        clipOverrides = newAnimationClip; // E depois só define que é igual ao substituto
    }
}
