#region License
/*
 *  Xna5D.Data.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: May 28, 2004
 */
#endregion

#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Xe.Data
{
    public class Tokenizer
    {
        // The current string to look at
        private string myString;

        // An array of tokens, based on the delimeter
        private string[] myTokens;

        // Index of the current token
        private int cToken;

        /// <summary>
        /// The Default Constructor that initializes an instance of the class.
        /// </summary>
        public Tokenizer()
        {
        }

        /// <summary>
        /// Initializes an instance with a string and a default delimeter.
        /// </summary>
        /// <param name="str">The string to be tokenized.</param>
        public Tokenizer(string str)
        {
            // Initialize the class.
            Tokenize(str);
        }

        /// <summary>
        /// Initializes an instance with a string and a delimeter.
        /// </summary>
        /// <param name="str">The string to be tokenized.</param>
        /// <param name="delimiter">The delimeter to tokenize the string with.</param>
        public Tokenizer(string str, char delimiter)
        {
            // Initialize the class with a specified delimeter.
            Tokenize(str, delimiter);
        }

        public void Tokenize(string str, char[] delimiter)
        {
            myString = str;					// We setup the class' string.

            myTokens = myString.Split(delimiter);	// Split the string and put it
            //	in myTokens.

            cToken = -1;					// Set the current token to -1.
        }

        /// <summary>
        /// Tokenizes a provided string.
        /// </summary>
        /// <param name="str">String to be tokenized.</param>
        public void Tokenize(string str)
        {
            char[] c = new char[1];			// This is the delimeter.

            myString = str;					// We setup the class' string.

            c[0] = ' ';						// Set the delimeter.
            myTokens = myString.Split(c);	// Split the string and put it
            //	in myTokens.

            cToken = -1;					// Set the current token to -1.
        }

        /// <summary>
        /// Tokenizes a provided string with a provided delimeter.
        /// </summary>
        /// <param name="str">String to be tokenized.</param>
        /// <param name="delimiter">Delimeter to tokenize the string with.</param>
        public void Tokenize(string str, char delimiter)
        {
            char[] c = new char[1];			// This is the delimeter.

            myString = str;					// Setup the string...

            c[0] = delimiter;				// Set the delimeter (user specified).
            myTokens = myString.Split(c);	// Split the array into myTokens.

            cToken = -1;					// Set the current token to -1.
        }

        /// <summary>
        /// Gets the rest of the string without tokenizing any of it.
        /// </summary>
        /// <returns>A String.</returns>
        public string getRest()
        {
            string temp = "";
            for (int i = cToken + 1; i < myTokens.Length; i++)
            {
                if (i + 1 >= myTokens.Length)
                    temp += myTokens[i].Trim();
                else
                    temp += myTokens[i].Trim() + " ";
            }

            return temp;
        }

        /// <summary>
        /// Peeks at the next token without incrementing the stack.
        /// </summary>
        /// <returns></returns>
        public string peek()
        {
            int _cToken;

            if (cToken < 0)
                _cToken = 0;
            else
                _cToken = cToken;

            if (cToken >= myTokens.Length)
                return "-1";

            return myTokens[_cToken].Trim();
        }

        /// <summary>
        /// Gets the next available token.
        /// </summary>
        /// <returns>A String.</returns>
        public string nextToken()
        {
            /*
             *    Basically if we still have more tokens, increase the index
             * and return the next token. Simple really. Otherwise return
             * an error code.
             */
            if (hasMoreTokens())
            {
                cToken++;
                return myTokens[cToken].Trim();
            }
            else
                return "-1";
        }

        /// <summary>
        /// Returns a boolean representing whether or not a string has more tokens.
        /// </summary>
        /// <returns>A Boolean.</returns>
        public bool hasMoreTokens()
        {
            // Check to see if the current index is less than
            //        one less than final index.
            return (cToken < myTokens.Length - 1);
        }

        /// <summary>
        /// Returns the amount of tokens a string has.
        /// </summary>
        /// <returns>An Integer.</returns>
        public int numTokens()
        {
            // Return the number of tokens.
            return myTokens.Length;
        }
    }
}
