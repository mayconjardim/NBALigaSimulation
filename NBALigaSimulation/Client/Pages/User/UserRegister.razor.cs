namespace NBALigaSimulation.Client.Pages.User
{
    partial class UserRegister
    {

        UserRegistration user = new UserRegistration();

        void HandleRegistration()
        {
            Console.WriteLine($"Registrando um user: {user.Username}");
        }

    }
}
