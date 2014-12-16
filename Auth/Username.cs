﻿// ***********************************************************************
// Assembly         : Rabbit
// Author           : Decagon
// Created          : 07-22-2014
// ***********************************************************************
// <copyright file="Username.cs" company="None">
//     Copyright 2014 (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Rabbit.Auth
{
    using System;
    using System.Security.Authentication;
    using PlayerIOClient;

    /// <summary>
    /// Class Username.
    /// </summary>
    
    public static class UserName
    {
        /// <summary>
        /// Authenticates using the specified email.
        /// </summary>
        /// <param name="userName">
        /// The user Name.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// A valid PlayerIOClient instance.
        /// </returns>
        public static Client Authenticate(string userName, string password)
        {
            var c = PlayerIO.QuickConnect.SimpleConnect(RabbitAuth.GameId, "guest", "guest");

            string userId;

            try
            {
                userId = c.BigDB.Load("usernames", userName)["owner"].ToString();
            }
            catch (NullReferenceException)
            {
                // if the username is a user id
                userId = userName;
            }

            int? i = null;

            if (userId.StartsWith("simple", StringComparison.CurrentCulture))
            {
                i = 6;
            }

            if (userId.StartsWith("kong", StringComparison.CurrentCulture))
            {
                i = 4;
            }

            if (userId.StartsWith("armor", StringComparison.CurrentCulture) || userId.StartsWith("mouse", StringComparison.CurrentCulture))
            {
                i = 5;
            }

            if (userId.StartsWith("fb", StringComparison.CurrentCulture))
            {
                i = 2;
            }

            if (i != null)
            {
                return PlayerIO.QuickConnect.SimpleConnect(
                    RabbitAuth.GameId,
                    userId.Substring((int)i, userId.Length - (int)i), // trim the type from the id
                    password);
            }

            throw new AuthenticationException("Unknown username.");
        }
    }
}