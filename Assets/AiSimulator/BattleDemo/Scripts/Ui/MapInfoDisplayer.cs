using System;
using System.Collections.Generic;
using UnityEngine;
using IndieDevTools.Maps;
using UnityEngine.UI;
using TMPro;

namespace IndieDevTools.Demo.BattleSimulator
{
    /// <summary>
    /// A component that shows a clicked map element's name and updated descriptions.
    /// </summary>
    public class MapInfoDisplayer : MonoBehaviour
    {
        [SerializeField, Header("Info")]
        GameObject infoContainer = null;

        [SerializeField]
        Image icon = null;

        [SerializeField]
        TextMeshProUGUI displayName = null;

        [SerializeField]
        TextMeshProUGUI description = null;

        [SerializeField, Header("Instructions")]
        GameObject instructionsContainer = null;

        [NonSerialized]
        IMapElement currentMapElement = null;
        bool HasCurrentElement => currentMapElement != null;

        [NonSerialized]
        SpriteRenderer currentSpriteRenderer = null;

        [NonSerialized]
        Color currentSpriteRendererInitialColor = Color.white;

        void Start()
        {
            Draw();
        }

        void Update()
        {
            DetectMouseClick();
        }

        void OnDestroy()
        {
            RemoveMapElementEventHandlers();
        }

        void DetectMouseClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseClick();
            }
        }

        void HandleMouseClick()
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            IMapElement mapElement = null;
            SpriteRenderer spriteRenderer = null;

            if (hitInfo.transform != null)
            {
                GameObject hitGameObject = hitInfo.transform.gameObject;
                mapElement = hitGameObject.GetComponent<IMapElement>();
                if (mapElement != null)
                {
                    spriteRenderer = hitGameObject.GetComponentInChildren<SpriteRenderer>();    
                }
            }

            SetCurrentSpriteRenderer(spriteRenderer);
            SetMapElement(mapElement);
        }

        // Highlights the given sprite renderer by making it darker.
        void SetCurrentSpriteRenderer(SpriteRenderer spriteRenderer)
        {
            if (currentSpriteRenderer != null)
            {
                float alpha = currentSpriteRenderer.color.a;
                Color color = currentSpriteRendererInitialColor;
                if (alpha < color.a)
                {
                    color.a = alpha;
                }
                
                currentSpriteRenderer.color = color;
                currentSpriteRenderer = null;
            }

            if (spriteRenderer == null) return;

            currentSpriteRenderer = spriteRenderer;
            currentSpriteRendererInitialColor = currentSpriteRenderer.color;

            Color.RGBToHSV(currentSpriteRenderer.color, out float h, out float s, out float v);
            v *= 0.5f;

            Color darkerColor = Color.HSVToRGB(h, s, v);
            currentSpriteRenderer.color = darkerColor;
        }

        void SetMapElement(IMapElement mapElement)
        {
            RemoveMapElementEventHandlers();
            currentMapElement = mapElement;
            AddMapElementEventHandlers();
            Draw();
        }

        void AddMapElementEventHandlers()
        {
            RemoveMapElementEventHandlers();
            if (currentMapElement == null) return;
            (currentMapElement as IDescribable).OnUpdated += CurrentMapElement_OnUpdated;
        }

        void RemoveMapElementEventHandlers()
        {
            if (currentMapElement == null) return;
            (currentMapElement as IDescribable).OnUpdated -= CurrentMapElement_OnUpdated;
        }

        private void CurrentMapElement_OnUpdated(IDescribable obj)
        {
            Draw();
        }

        void Draw()
        {
            if (HasCurrentElement)
            {
                DrawInfo();
            }

            SetContainersActive();
        }

        void DrawInfo()
        {
            if (HasCurrentElement == false) return;

            displayName.text = currentMapElement.DisplayName;
            description.text = currentMapElement.Description;

            icon.sprite = currentSpriteRenderer.sprite;
            icon.color = currentSpriteRendererInitialColor; 
        }

        void SetContainersActive()
        {
            infoContainer.SetActive(HasCurrentElement);
            instructionsContainer.SetActive(!HasCurrentElement);
        }
    }
}
