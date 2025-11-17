using System.Collections;
using Fusion;
using TMPro;
using UnityEngine;

public class ColorManager : NetworkBehaviour
{
    public static ColorManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI _teamOneText;
    [SerializeField] private TextMeshProUGUI _teamTwoText;
    //[Networked, OnChangedRender(nameof(UpdateText))] TextMeshProUGUI _teamOneText { get; set; }
    //[Networked, OnChangedRender(nameof(UpdateText))] TextMeshProUGUI _teamTwoText { get; set; }


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
        //UpdateText(_teamOneText, TeamOneFalls);
        //UpdateText(_teamTwoText, TeamTwoFalls);

        UpdateText();

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

        //UpdateText(_teamOneText, TeamOneFalls);
        //UpdateText(_teamTwoText, TeamTwoFalls);
        UpdateText();
    }

    public void UpdateText()
    {
        _teamOneText.text = $"{TeamOneFalls}";
        _teamTwoText.text = $"{TeamTwoFalls}";
    }

    public Color GetWinner()
    {
        return TeamOneFalls < TeamTwoFalls ? _teamOneColor : _teamTwoColor;
    }

    private void Restart()
    {
        TeamOneFalls = 0;
        TeamTwoFalls = 0;

        UpdateText();
        //UpdateText(_teamOneText, TeamOneFalls);
        //UpdateText(_teamTwoText, TeamTwoFalls);
    }
}
