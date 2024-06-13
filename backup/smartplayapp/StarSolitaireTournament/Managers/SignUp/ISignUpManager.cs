using Models;

namespace StarSolitaireTournament.Managers
{
    public interface ISignUpManager
    {
        Task<SignUpResult> SetPlayerNameAndProfile(string googleSignInUserId, string playerName, string profileURL, string email);
    }
}