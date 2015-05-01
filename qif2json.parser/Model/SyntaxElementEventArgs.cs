// Created by: egr
// Created at: 01.05.2015
// © 2015 Alexander Egorov

using System;

namespace qif2json.parser.Model
{
    /// <summary>
    /// Element detection results
    /// </summary>
    public sealed class SyntaxElementEventArgs : EventArgs
    {
        private readonly string element;

        /// <summary>
        /// Initializes new args instance
        /// </summary>
        /// <param name="element">Syntax element detected</param>
        public SyntaxElementEventArgs(string element)
        {
            this.element = element;
        }

        public string Element
        {
            get { return this.element; }
        }
    }
}