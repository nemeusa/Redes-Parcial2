using System.Collections;
using Fusion;
using TMPro;
using UnityEngine;

public class ColorManager : NetworkBehaviour
{
    public static ColorManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI _teamOneText;
    [SerializeField] private TextMeshProUGUI _teamTwoText;

    public int TeamOneFalls { get; private set; }
    public int TeamTwoFalls { get; private set; }

    private Color _teamOneColor;
    private Color _teamTwoColor;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateText(_teamOneText, TeamOneFalls);
        UpdateText(_teamTwoText, TeamTwoFalls);

        _teamOneColor = GameManager.Instance.TeamOneColor;
        _teamTwoColor = GameManager.Instance.TeamTwoColor;

        GameManager.Instance.OnStartGame += Restart;
    }

    public void AddBlock(EnemyLife player)
    {
        if (player.HasInputAuthority)
        {
            TeamOneFalls++;
        }
        else
        {
            TeamTwoFalls++;
        }

        UpdateText(_teamOneText, TeamOneFalls);
        UpdateText(_teamTwoText, TeamTwoFalls);
    }

    private void UpdateText(TextMeshProUGUI t, int num)
    {
        t.text = $"{num}";
    }

    public Color GetWinner()
    {
        return TeamOneFalls < TeamTwoFalls ? _teamOneColor : _teamTwoColor;
    }

    private void Restart()
    {
        TeamOneFalls = 0;
        TeamTwoFalls = 0;

        UpdateText(_teamOneText, TeamOneFalls);
        UpdateText(_teamTwoText, TeamTwoFalls);
    }
}
