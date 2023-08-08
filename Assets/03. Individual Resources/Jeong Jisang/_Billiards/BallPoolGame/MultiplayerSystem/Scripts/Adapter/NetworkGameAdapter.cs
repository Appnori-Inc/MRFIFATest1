using NetworkManagement;

/// <summary>
/// Network and game adapter.
/// </summary>
public interface NetworkGameAdapter
{
    void SetTurn(int turnId);
    void OnMainPlayerLoaded (int playerId, string name, int coins, object avatar, string avatarURL, int prize);
	void OnUpdateMainPlayerName (string name);
}
