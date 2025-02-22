// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

namespace UnityEngine.UIElements
{
    /// <summary>
    /// Use this class to display a contextual menu.
    /// </summary>
    public abstract class ContextualMenuManager
    {
        // Case 1268159 - Work around for events registered on mouse down, that want to modify the UI. GenericMenu freezes
        // the editor UI when shown, hence latest changes are not displayed if they occured on the frame. So we need to
        // allow to work in 2 steps on Mac. On windows it can already work because menu is displayed on mouse up.
        internal bool displayMenuHandledOSX { get; set; }

        /// <summary>
        /// Checks if the event triggers the display of the contextual menu. This method also displays the menu.
        /// </summary>
        /// <param name="evt">The event to inspect.</param>
        /// <param name="eventHandler">The element for which the menu is displayed.</param>
        /// <remarks>
        /// This is an abstract method. Each concrete implementation of the ContextualMenuManager defines the events that will display the menu.
        /// </remarks>
        public abstract void DisplayMenuIfEventMatches(EventBase evt, IEventHandler eventHandler);

        /// <summary>
        /// Displays the contextual menu.
        /// </summary>
        /// <param name="triggerEvent">The event that triggered the display of the menu.</param>
        /// <param name="target">The element for which the menu is displayed.</param>
        /// <returns>True if a contextual menu was effectively displayed.</returns>
        public void DisplayMenu(EventBase triggerEvent, IEventHandler target)
        {
            DropdownMenu menu = new DropdownMenu();

            using (ContextualMenuPopulateEvent cme = ContextualMenuPopulateEvent.GetPooled(triggerEvent, menu, target, this))
            {
                target?.SendEvent(cme);
            }

            if (UIElementsUtility.isOSXContextualMenuPlatform)
            {
                displayMenuHandledOSX = true;

                // The pointer up will likely happen on the menu itself. Since we won't get a chance to call
                // ActivateCompatibilityMouseEvents until the next pointer down, we need to not prevent them.
                if (triggerEvent is PointerDownEvent p)
                    p.DontPreventCompatibilityMouseEventsOnPropagationStopped();
            }
        }

        /// <summary>
        /// Displays the contextual menu.
        /// </summary>
        /// <param name="menu">The menu to display.</param>
        /// <param name="triggerEvent">The event that triggers the display of the contextual menu.</param>
        protected internal abstract void DoDisplayMenu(DropdownMenu menu, EventBase triggerEvent);
    }
}
