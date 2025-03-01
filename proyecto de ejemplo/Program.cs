// Program.cs
using System;
using System.Text.RegularExpressions;

IPasswordValidator validator = new PasswordValidator();

Console.WriteLine("🔐 Validador de Contraseñas .NET 9 🔐");
Console.WriteLine("Requisitos:");
Console.WriteLine("- 8+ caracteres");
Console.WriteLine("- Al menos una mayúscula y una minúscula");
Console.WriteLine("- Al menos un número");
Console.WriteLine("- Un carácter especial (@$!%*?&)");
Console.WriteLine("\nEscribe 'salir' para terminar\n");

while (true)
{
    Console.Write("Ingresa contraseña: ");
    var input = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(input) || input.Equals("salir", StringComparison.OrdinalIgnoreCase))
        break;

    var isValid = validator.IsValid(input);

    Console.ForegroundColor = isValid ? ConsoleColor.Green : ConsoleColor.Red;
    Console.WriteLine(isValid ? "✅ VÁLIDA" : "❌ NO VÁLIDA");
    Console.ResetColor();
}

public interface IPasswordValidator
{
    bool IsValid(string? password);
}

public class PasswordValidator : IPasswordValidator
{
    public bool IsValid(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        return new Regex(@".{8,}").IsMatch(password) &&        // Longitud
               new Regex(@"[A-Z]").IsMatch(password) &&        // Mayúscula
               new Regex(@"[a-z]").IsMatch(password) &&        // Minúscula
               new Regex(@"[0-9]").IsMatch(password) &&       // Número
               new Regex(@"[@$!%*?&]").IsMatch(password);      // Carácter especial
    }
}