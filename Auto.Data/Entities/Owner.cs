﻿#nullable enable
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Auto.Data.Entities;

public class Owner
{
    public Owner()
    {
    }
    public Owner(string firstName, string middleName, string lastName, string email)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Email = email;
    }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    
    public string Email { get; set; }
    
    [Newtonsoft.Json.JsonIgnore] 
    public Vehicle? Vehicle { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public string GetFullName => $"{FirstName}&{MiddleName}&{LastName}";
}