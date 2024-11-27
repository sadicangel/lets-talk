using LetsTalk.WebApi.Entities;
using Microsoft.AspNetCore.Identity;

namespace LetsTalk.WebApi.Services;

public sealed class PasswordHasher : PasswordHasher<User>;
