using System;
using System.Collections.Generic;
using UnityEngine;
using IndieDevTools.Maps;
using UnityEngine.UI;
using TMPro;

namespace IndieDevTools.Demo.CrabBattle
{
    /// <summary>
    /// A component that shows the status of a crab agent.
    /// </summary>
    public class CrabInfoDisplayer : MonoBehaviour
    {
        /// <summary>
        /// The map to find crab agents in.
        /// </summary>
        IMap Map => map;
        [SerializeField]
        AbstractMap map = null;
        
        /// <summary>
        /// The game object container for the info UI elements
        /// </summary>
        [SerializeField, Header("Info")]
        GameObject infoContainer = null;

        /// <summary>
        /// The clicked agent's icon
        /// </summary>
        [SerializeField]
        Image icon = null;

        /// <summary>
        /// The clicked agent's display name.
        /// </summary>
        [SerializeField]
        TextMeshProUGUI displayName = null;

        /// <summary>
        /// The clicked agent's description
        /// </summary>
        [SerializeField]
        TextMeshProUGUI description = null;

        /// <summary>
        /// The game object container for the instrucion UI elements.
        /// </summary>
        [SerializeField, Header("Instructions")]
        GameObject instructionsContainer = null;

        /// <summary>
        /// The currently selected map element.
        /// </summary>
        [NonSerialized]
        IMapElement currentMapElement = null;

        /// <summary>
        /// Whether or not there is an actively selected map element.
        /// </summary>
        bool HasCurrentElement => currentMapElement != null;

        /// <summary>
        /// The currently selected sprite renderer.
        /// </summary>
        [NonSerialized]
        SpriteRenderer currentSpriteRenderer = null;

        /// <summary>
        /// The initial color of the selected sprite renderer.
        /// </summary>
        [NonSerialized]
        Color currentSpriteRendererInitialColor = Color.white;

        /// <summary>
        /// Unity start event method that calls the draw method.
        /// </summary>
        void Start() => Draw();

        /// <summary>
        /// Unity update event method that checks if the mouse was clicked.
        /// </summary>
        void Update() => DetectMouseClick();

        /// <summary>
        /// Unity destroy event method that removes map element event handlers.
        /// </summary>
        void OnDestroy() => RemoveMapElementEventHandlers();

        /// <summary>
        /// Detects if the user has clicked the mouse.
        /// </summary>
        void DetectMouseClick()
        {
            if (Input.GetMouseButtonDown(0)) HandleMouseClick();
        }

        /// <summary>
        /// Mouse click handler that sets the selected elements.
        /// </summary>
        void HandleMouseClick()
        {
            Vector3 worldPosition = Input.mousePosition;
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(worldPosition);
            Vector2Int cell = Map.LocalToCell(worldPoint);

            ICrab crab = Map.GetMapElementAtCell<ICrab>(cell);

            SetCurrentSpriteRenderer(crab?.SpriteRenderer);
            SetMapElement(crab);
        }

        /// <summary>
        /// Sets and highlights the given sprite renderer by making it darker.
        /// </summary>
        /// <param name="spriteRenderer">A sprite renderer</param>
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

        /// <summary>
        /// Resets the currently selected sprite renderer to its initial color.
        /// </summary>
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

        /// <summary>
        /// Sets the map element and initializes its event handlers.
        /// </summary>
        /// <param name="mapElement">A map element</param>
        void SetMapElement(IMapElement mapElement)
        {
            RemoveMapElementEventHandlers();
            currentMapElement = mapElement;
            AddMapElementEventHandlers();
            Draw();
        }

        /// <summary>
        /// Adds event handler to the currently selected map element.
        /// </summary>
        void AddMapElementEventHandlers()
        {
            RemoveMapElementEventHandlers();
            if (currentMapElement == null) return;
            (currentMapElement as IDescribable).OnUpdated += CurrentMapElement_OnUpdated;
        }

        /// <summary>
        /// Removes event handlers from the currently selected map element.
        /// </summary>
        void RemoveMapElementEventHandlers()
        {
            if (currentMapElement == null) return;
            (currentMapElement as IDescribable).OnUpdated -= CurrentMapElement_OnUpdated;
        }

        /// <summary>
        /// The event handler for the currently selected map element that redraws the info UI on updates. 
        /// </summary>
        /// <param name="obj"></param>
        private void CurrentMapElement_OnUpdated(IDescribable obj)
        {
            Draw();
        }

        /// <summary>
        /// Draws info if there is currently selected element.
        /// </summary>
        void Draw()
        {
            if (HasCurrentElement)
            {
                DrawInfo();
            }

            SetContainersActive();
        }

        /// <summary>
        /// Draws the name, description and icon of the selected crab.
        /// </summary>
        void DrawInfo()
        {
            if (currentMapElement == null) return;
            displayName.text = currentMapElement.DisplayName;
            description.text = currentMapElement.Description;

            if (currentSpriteRenderer == null) return;
            icon.sprite = currentSpriteRenderer.sprite;
            icon.color = currentSpriteRendererInitialColor; 
        }

        /// <summary>
        /// Sets the info UI active if there is a currently selected element.
        /// Otherwise, show the instructions UI.
        /// </summary>
        void SetContainersActive()
        {
            infoContainer.SetActive(HasCurrentElement);
            instructionsContainer.SetActive(!HasCurrentElement);
        }
    }
}
