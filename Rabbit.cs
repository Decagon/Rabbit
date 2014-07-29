﻿// ***********************************************************************
// Assembly         : Rabbit
// Author           : Decagon
// Created          : 07-22-2014
// ***********************************************************************
// <copyright file="Rabbit.cs" company="None">
//     Copyright 2014 (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Rabbit
{
    using System;
    using System.Text.RegularExpressions;

    using PlayerIOClient;

    using global::Rabbit.Auth;

    /// <summary>
    /// Authentication core.
    /// </summary>
    public class Rabbit
    {
        /// <summary>
        /// The stored server version.
        /// </summary>
        public const int StoredVersion = 179;

        /// <summary>
        /// The game identifier
        /// </summary>
        public const string GameId = "everybody-edits-su9rn58o40itdbnw69plyw";

        /// <summary>
        /// Gets or sets the Client for the main authentication system.
        /// </summary>
        /// <value>The client.</value>
        private Client Client { get; set; }

        /// <summary>
        /// Gets or sets the PlayerIO connection to the server.
        /// </summary>
        /// <value>The everybody edits connection.</value>
        private Connection EeConn { get; set; }

        /// <summary>
        /// Gets the type of the authentication.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <returns>The authentication type.</returns>
        /// <exception cref="System.InvalidOperationException">Invalid authentication type.</exception>
        public static AuthType GetAuthType(string email, string password)
        {

            // ArmorGames: Both UserID and password are 32 char hexadecimal lowercase strings
            if (!string.IsNullOrEmpty(email) &&
                Regex.IsMatch(password, @"^[0-9a-f]{32}$") &&
                Regex.IsMatch(email, @"^[0-9a-f]{32}$"))
            {
                return AuthType.ArmorGames;
            }

            // Kongregate: 
            // UserID is a number
            // Password is a 64 char hexadecimal lowercase string
            if (!string.IsNullOrEmpty(email) &&
                Regex.IsMatch(email, @"^\d+$") &&
                Regex.IsMatch(password, @"^[0-9a-f]{64}$"))
            {
                return AuthType.Kongregate;
            }

            // Facebook: password is a 100 char alphanumerical string
            // there is no UserID supplied
            if (string.IsNullOrEmpty(email) &&
                Regex.IsMatch(password, @"^[0-9a-z]{100,}$", RegexOptions.IgnoreCase))
            {
                return AuthType.Facebook;
            }

            if (!string.IsNullOrEmpty(email) &&
                !string.IsNullOrEmpty(password) &&
                IsValidEmail(email))
            {
                return AuthType.Regular;
            }

            throw new InvalidOperationException("Invalid authentication type.");
        }

        // TODO: support SecureStrings and reverse password and email parameters.

        /// <summary>
        /// Connects to the PlayerIO service using the provided credentials.
        /// </summary>
        /// <param name="email">
        /// Email address
        /// </param>
        /// <param name="worldId">
        /// The room id of the world to join
        /// </param>
        /// <param name="password">
        /// Password or token
        /// </param>
        /// <param name="createRoom">
        /// Whether or not to create a room or join an existing one.
        /// </param>
        /// <param name="authType">
        /// The authentication type.
        /// </param>
        /// <returns>
        /// A valid connection object.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// Invalid authentication type.
        /// </exception>
        public Connection LogIn(string email, string worldId, string password = null, bool createRoom = true, AuthType authType = AuthType.Unknown)
        {
            // Clean the email (or token) from whitespace
            email = Regex.Replace(email, @"\s+", string.Empty);

            // Parse the world id (if it exists in another format)
            worldId = IdParser.Parse(worldId);

            // backwards compatibility
            if (authType == AuthType.Unknown)
            {
                authType = GetAuthType(email, password);
            }

            switch (authType)
            {
                case AuthType.Facebook:
                {
                    Client = Facebook.Authenticate(password);
                    break;
                }

                case AuthType.Kongregate:
                {
                    Client = Kongregate.Authenticate(email, password);
                    break;
                }

                case AuthType.ArmorGames:
                {
                    Client = ArmorGames.Authenticate(email, password);
                    break;
                }

                default:
                    {
                        Client = PlayerIO.QuickConnect.SimpleConnect(GameId, email, password);
                        break;
                }
            }

            if (createRoom)
            {
                var roomPrefix = worldId.StartsWith("BW") 
                    ? "Beta"
                    : "Everybodyedits";

                var serverVersion = Client.BigDB.Load("config", "config")["version"];
                this.EeConn = Client.Multiplayer.CreateJoinRoom(
                    worldId,
                    roomPrefix + serverVersion,
                    true,
                    null,
                    null);

                if (Convert.ToInt32(serverVersion) <= StoredVersion)
                {
                    return this.EeConn;
                }

                const string ErrorMsg = "Rabbit: WARNING the server version is greater than the version Rabbit is compatible with." +
                                        " Consider updating Rabbit to the latest version at https://github.com/Decagon/Rabbit/releases";
                Console.WriteLine(ErrorMsg);
                System.Diagnostics.Debug.Write(ErrorMsg);
            }
            else
            {
                this.EeConn = Client.Multiplayer.JoinRoom(
                    worldId,
                    null);
            }

            return this.EeConn;
        }

        /// <summary>
        /// Check if the email is valid.
        /// </summary>
        /// <param name="strIn">
        /// The string (email).
        /// </param>
        /// <returns>
        /// Whether or not the email is valid.
        /// </returns>
        internal static bool IsValidEmail(string strIn) // http://stackoverflow.com/questions/5342375/
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
    }
}
