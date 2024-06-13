using Models;
using System.Threading.Tasks;


public interface IUserQueries
{
	Task<int> InsertUser(string firebaseToken, string googleSignInUserId, string username, string profileUrl, bool isBot, string email);
	Task GetUser(int userId);
	Task<UserDetails> GetUserDetails(int userId);
	Task<bool> IsGoogleUserIdExists(string phoneNumber);
	Task<string> GetUserName(int userId);
	Task<UsernameAndProfileURL> GetUserNameAndProfileUrl(int userId);
	Task<string> GetEmailId(int userId);
	Task<string> GetUserProfileURL(int userId);
	Task<int> GetUserIdByGoogleUserId(string googleUserId);
	Task<CachedUserDetails> GetUserDetailsForCache(int id);
	Task<int> GetTotalPlayers();
	Task<int> UpdateUsernameAndProfileURL(int userId, string username, string newProfileURL);
	Task<int> UpdateUserProfileURL(int userId, string newProfileURL);
	Task<int> UpdateUserFirebaseToken(int userId, string firebaseToken);
	Task<int> UpdateUserEmail(int userId, string email);
	Task DeleteUser(int userId);
}
