﻿// ***********************************************************************
// Assembly         : Rabbit
// Author           : Decagon
// Created          : 07-22-2014
// ***********************************************************************
// <copyright file="RabbitAuth.cs" company="None">
//     Copyright 2014 (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Text.RegularExpressions;
using Rabbit.Auth;
using Rabbit.Localizations;

namespace Rabbit
{
    /// <summary>
    ///     Authentication core.
    /// </summary>
    public class RabbitAuth
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RabbitAuth" /> class.
        /// </summary>
        public RabbitAuth()
        {
            AuthenticationType = AuthenticationType.Unknown;
        }

        /// <summary>
        ///     Gets or sets the authentication type.
        /// </summary>
        /// <value>The type of the authentication.</value>
        public AuthenticationType AuthenticationType { get; set; }

        /// <summary>
        ///     Gets the type of the authentication.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <returns>The authentication type.</returns>
        /// <exception cref="System.InvalidOperationException">Invalid authentication type.</exception>
        public static AuthenticationType GetAuthType(string email, string password)
        {
            // Armor Games and Kongregate require that the email field is not blank.
            if (!string.IsNullOrEmpty(email))
            {

                // Armor Games and Kongregate require that the password is hexadecimal

                if (IsHexadecimal(password))
                {
                    // Armor Games:
                    // Username: a 32 character lowercase hexadecimal string
                    // Password: a 32 character lowercase hexadecimal string
                    if (password.Length == 32 &&
                        IsHexadecimal(email, 32))
                    {
                        return AuthenticationType.ArmorGames;
                    }

                    // Kongregate: 
                    // Username: a positive integer
                    // Password: a 64 character lowercase hexadecimal string
                    if (Regex.IsMatch(email, @"^\d+$") &&
                        password.Length == 64)
                    {
                        return AuthenticationType.Kongregate;
                    }
                }
                // Mousebreaker:
                // Username: a valid email address
                // Password: 88 character base 64 string
                if (!string.IsNullOrEmpty(password))
                {
                    if (password.Length == 88)
                    {
                        try
                        {
                            Convert.FromBase64String(password);
                            return AuthenticationType.Mousebreaker;
                        }
                        catch (FormatException)
                        {
                            // safe to ignore the exception because it is not a valid
                            // base 64 array.
                        }
                    }
                    // otherwise, let's hope it's regular authentication.
                    return IsValidEmail(email) ? AuthenticationType.Regular : AuthenticationType.Username;
                }
            }
            else
            {
                // the email and password cannot both be blank
                if (string.IsNullOrEmpty(password))
                {
                    throw new InvalidOperationException(strings.EmailPasswordNullError);
                }

                // Facebook:
                // Username: N/A
                // Password: 100 character (or greater) alphanumerical string
                if (Regex.IsMatch(password, @"^[0-9a-z]{100,}$", RegexOptions.IgnoreCase))
                {
                    return AuthenticationType.Facebook;
                }
            }


            // Try to help the user if they entered in invalid data.
            // Guess what possible authentication type they might be trying to
            // use and tell them that there is a proper way to format it.
            throw new InvalidOperationException(Errors.GenerateErrorMessage(email, password));
        }

        /// <summary>
        ///     Determines whether the specified password is hexadecimal.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="length">The length.</param>
        /// <returns><c>true</c> if the specified password is hexadecimal; otherwise, <c>false</c>.</returns>
        internal static bool IsHexadecimal(string password, int length = 0)
        {
            return length != 0
                ? Regex.IsMatch(password, @"^[0-9a-f]{" + length + "}$")
                : Regex.IsMatch(password, @"^[0-9a-f]$");
        }

        /// <summary>
        ///     Connects to the PlayerIO service using the provided credentials.
        /// </summary>
        /// <param name="gameId">The game id.</param>
        /// <param name="email">Email address.</param>
        /// <param name="password">Password or token.</param>
        /// <param name="shouldUseSecureApiRequests">if set to <c>true</c> secure API requests will be used.</param>
        /// <returns>A client object.</returns>
        /// <exception cref="System.InvalidOperationException">Invalid authentication type.</exception>
        public Client LogOn(string gameId, string email, string password, bool shouldUseSecureApiRequests = false)
        {
            PlayerIO.UseSecureApiRequests = shouldUseSecureApiRequests;

            // Clean the email (or token), and gameId from whitespace
            email = Regex.Replace(email, @"\s+", string.Empty);
            gameId = Regex.Replace(gameId, @"\s+", string.Empty);


            if (!Regex.IsMatch(gameId, @"^[0-9a-zA-Z\-]+$"))
            {
                throw new ArgumentException("The game ID contains an invalid character.", "gameId");
            }


            if (AuthenticationType == AuthenticationType.Unknown)
            {
                AuthenticationType = GetAuthType(email, password);
            }

            switch (AuthenticationType)
            {
                case AuthenticationType.Facebook:
                {
                    return Facebook.Authenticate(gameId, password);
                }

                case AuthenticationType.Kongregate:
                {
                    return Kongregate.Authenticate(gameId, email, password);
                }

                case AuthenticationType.ArmorGames:
                {
                    return ArmorGames.Authenticate(gameId, email, password);
                }

                case AuthenticationType.Mousebreaker:
                {
                    return MouseBreaker.Authenticate(gameId, email, password);
                }

                case AuthenticationType.Username:
                {
                    return Username.Authenticate(gameId, email, password);
                }

                default:
                {
                    return Simple.Authenticate(gameId, email, password);
                }
            }
        }

        /// <summary>
        ///     The log in function.
        /// </summary>
        /// <param name="gameId">The game id.</param>
        /// <param name="token">The token.</param>
        /// <param name="shouldUseSecureApiRequests">if set to <c>true</c> secure API requests will be used.</param>
        /// <returns>
        ///     The <see cref="Client" />.
        /// </returns>
        public Client LogOn(string gameId, string token, bool shouldUseSecureApiRequests = false)
        {
            return LogOn(gameId, token, null, shouldUseSecureApiRequests);
        }

        /// <summary>
        ///     Check if the email is valid.
        /// </summary>
        /// <param name="strIn">The string (email).</param>
        /// <returns>Whether or not the email is valid.</returns>
        internal static bool IsValidEmail(string strIn) // http://stackoverflow.com/questions/5342375/
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn,
                @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
    }
}