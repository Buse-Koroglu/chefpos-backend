namespace ChefPos.Application.Common.Interfaces;

public interface IInitialPasswordGenerator
{
    string Generate(string firstName, string personalId);
}