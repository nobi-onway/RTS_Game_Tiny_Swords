using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/ActionSO/TrainUnitAction", fileName = "ActionSO")]
public class TrainUnitActionSO : ActionSO
{
    [SerializeField] private Unit m_unitPrefab;
    [SerializeField] private int m_goldCost;
    [SerializeField] private int m_woodCost;
    [SerializeField] private string m_description;

    public Unit UnitPrefab => m_unitPrefab;
    public int GoldCost => m_goldCost;
    public int WoodCost => m_woodCost;
    public string Description => m_description;

    public override void PrepareExecute()
    {
        UIManager.Instance.ShowConfirmationBar(GoldCost, WoodCost, Description, FinalizeUnitTraining, UIManager.Instance.HideConfirmationBar);
    }

    private void FinalizeUnitTraining()
    {
        if (!PlayerResourceManager.Instance.TryReduceResource(GoldCost, WoodCost)) return;

        Collider2D castleCollider = GameManager.Instance.ActiveUnit.Collider;
        float spawnRadius = castleCollider.bounds.extents.x;

        int maxSpawnAttempts = 40;

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            float angle = (360f / maxSpawnAttempts) * i;

            Vector2 spawnOffset = new Vector2(Mathf.Cos(angle * Mathf.Rad2Deg), Mathf.Sin(angle * Mathf.Rad2Deg)) * spawnRadius;
            Vector2 spawnPosition = (Vector2)GameManager.Instance.ActiveUnit.transform.position + spawnOffset;

            Collider2D[] hits = Physics2D.OverlapCircleAll(spawnPosition, 0.1f).Where(hit => hit.TryGetComponent(out HumanoidUnit unit)).ToArray();
            bool isPositionOccupied = hits.Length > 0;

            bool canWalkable = TilemapManager.Instance.IsInWalkable(GridUtils.SnapToGrid(spawnPosition));

            if (!isPositionOccupied && canWalkable)
            {
                PlayerResourceManager.Instance.ReduceResource(GoldCost, WoodCost);
                Instantiate(m_unitPrefab, spawnPosition, Quaternion.identity);

                return;
            }
        }

        UIManager.Instance.HideConfirmationBar();
    }
}