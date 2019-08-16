using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SharedComponents.XML
{
    /// <summary>
    /// XElement extension helps with getting element/s from specific path e.g. "element1/element2/element3"
    /// </summary>
    public static class XElementExtension
    {
        /// <summary>
        /// Gets list of elements for specific path.
        /// </summary>
        /// <param name="element">Parent element.</param>
        /// <param name="path">Path to chosen elements.</param>
        /// <exception cref="ArgumentNullException"> If element is null.</exception>
        /// <exception cref="InvalidOperationException">If some element on path does not exist.</exception>
        /// <returns></returns>
        public static List<XElement> GetElements(this XElement element, string path)
        {
            if (element == null)
            {
                throw new ArgumentNullException("Parameter element may not be null.");
            }
            List<string> elements = path.Split('/').ToList();
            XElement temporaryElement = null;
            for (int i = 0; i < elements.Count; i++)
            {
                if (i == 0)
                {
                    temporaryElement = element;
                }

                if (temporaryElement == null)
                {
                    throw new InvalidOperationException(string.Format("Element {0} does not exists.", elements[i - 1]));
                }

                if (i == elements.Count - 1)
                {
                    return temporaryElement.Elements(elements[i]).ToList();
                }
                temporaryElement = temporaryElement.Element(elements[i]);
            }
            return new List<XElement>();
        }

        /// <summary>
        /// Gets element for specific path.
        /// </summary>
        /// <param name="element">Parent element</param>
        /// <param name="path">Path to chosen element.</param>
        /// <exception cref="ArgumentNullException"> If element is null.</exception>
        /// <exception cref="InvalidOperationException">If some element on path does not exist.</exception>
        /// <returns></returns>
        public static XElement GetElement(this XElement element, string path)
        {
            if (element == null)
            {
                throw new ArgumentNullException("Parameter element may not be null.");
            }
            List<string> elements = path.Split('/').ToList();
            XElement temporaryElement = null;
            for (int i = 0; i < elements.Count; i++)
            {
                if (i == 0)
                {
                    temporaryElement = element;
                }
                if (temporaryElement == null)
                {
                    throw new InvalidOperationException(string.Format("Element {0} does not exists.", elements[i - 1]));
                }
                if (i == elements.Count - 1)
                {
                    return temporaryElement.Element(elements[i]);
                }
                temporaryElement = temporaryElement.Element(elements[i]);
            }
            return null;
        }

        /// <summary>
        /// Gets element with specific attribute.
        /// </summary>
        ///<param name="element">Parent element.</param>
        /// <param name="pathFromElement">Path from element.</param>
        /// <param name="attributeName">Attribute name.</param>
        /// <param name="attributeValue">Attribute value.</param>
        /// <returns>Found element</returns>
        /// <returns></returns>
        public static  XElement GetElementWithSpecificAttribute(this XElement element, string pathFromElement, string attributeName,
            string attributeValue)
        {
            XElement childElement = element.GetElements(pathFromElement).FirstOrDefault(el => (string)el.Attribute(attributeName) == attributeValue);
            return childElement == default(XElement) ? null : childElement;
        }

        /// <summary>
        /// Checks whether element exists or not.
        /// </summary>
        ///<param name="element">Parent element.</param>
        /// <param name="pathFromElement">Path from element.</param>
        /// <returns>True if element exists, otherwise false.</returns>
        public static bool ExistsElement(this XElement element, string pathFromElement)
        {
            return element.GetElements(pathFromElement).Any();
        }

        /// <summary>
        /// Checks whether element with specific attribute exists or not.
        /// </summary>
        ///<param name="element">Parent element.</param>
        /// <param name="pathFromElement">Path from element.</param>
        /// <param name="attributeName">Attribute name.</param>
        /// <param name="attributeValue">Attribute value.</param>
        /// <returns>True if element exists, otherwise false.</returns>
        public static bool ExistsElementWithSpecificAttribute(this XElement element, string pathFromElement, string attributeName,
            string attributeValue)
        {
            XElement childElement = element.GetElements(pathFromElement).FirstOrDefault(el => (string)el.Attribute(attributeName) == attributeValue);
            return childElement != default(XElement);
        }
    }
}
