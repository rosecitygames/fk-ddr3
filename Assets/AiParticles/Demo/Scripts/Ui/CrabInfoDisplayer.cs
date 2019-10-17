using System;
using System.Collections.Generic;
using UnityEngine;
using IndieDevTools.Maps;
using UnityEngine.UI;
using TMPro;

namespace IndieDevTools.Demo.CrabBattle
{
    /// <summary>
    /// A component that shows a clicked map element's name and updated descriptions.
    /// </summary>
    public class CrabInfoDisplayer : MonoBehaviour
    {
        [SerializeField]
        AbstractMap map = null;
        IMap Map => map;

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
            if (Input.GetMouseButtonDown(0)) HandleMouseClick();
        }

        void HandleMouseClick()
        {
            Vector3 worldPosition = Input.mousePosition;
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(worldPosition);
            Vector2Int cell = Map.LocalToCell(worldPoint);

            ICrab crab = Map.GetMapElementAtCell<ICrab>(cell);

            SetCurrentSpriteRenderer(crab?.SpriteRenderer);
            SetMapElement(crab);
        }

        // Highlights the given sprite renderer by making it darker.
        void SetCurrentSpriteRenderer(SpriteRenderer spriteRenderer)
        {
            ResetCurrentSpriteRenderer();

            if (spriteRenderer == null) return;

            currentSpriteRenderer = spriteRenderer;
            currentSpriteRendererInitialColor = currentSpriteRenderer.color;

            Color.RGBToHSV(currentSpriteRenderer.color, out float h, out float s, out float v);
            v *= 0.5f;

            Color darkerColor = Color.HSVToRGB(h, s, v);
            currentSpriteRenderer.color = darkerColor;
        }

        void ResetCurrentSpriteRenderer()
        {
            if (currentSpriteRenderer == null) return;

            float alpha = currentSpriteRenderer.color.a;
            Color color = currentSpriteRendererInitialColor;
            if (alpha < color.a)
            {
                color.a = alpha;
            }

            currentSpriteRenderer.color = color;
            currentSpriteRenderer = null;
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
