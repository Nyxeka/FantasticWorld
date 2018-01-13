using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon {

    // for now, we'll set up one attack move, with one transform delta (the sword).

    public Vector3 defaultPos;
    public Vector3 defaultRot;

    AttackMove mainAttack;

    public AnimationCurve toRearUp;
    public AnimationCurve toStrike;
    public AnimationCurve toRelease;

    public override void InitWeapon()
    {

        TransformDelta swordAttack = new TransformDelta
        {
            pose1 = new Vector3(-0.25f, 1.75f, -0.1f),
            pose2 = new Vector3(0.296f, 0.94f, 0.149f),

            rot1 = Quaternion.Euler(-58.6f, -26.7f, 43.7f),
            rot2 = Quaternion.Euler(146.696f, -36.06f, -62.05f),

            toRearUpTime = 0.41f,
            toStrikeTime = 0.2f,
            toReleaseTime = 0.7f,

            toRearUp = toRearUp,
            toStrike = toStrike,
            toRelease = toRelease,

            rotInclude = true
        };

        TransformDelta chestMove = new TransformDelta()
        {

            pose1 = new Vector3(0.0f, 2.1f, 0.8f),
            pose2 = new Vector3(0.0f, 0.6f, 1.0f),

            toRearUpTime = 0.4f,
            toStrikeTime = 0.2f,
            toReleaseTime = 0.7f,

            toRearUp = toRearUp,
            toStrike = toStrike,
            toRelease = toRelease,

            rotInclude = false

        };

        mainAttack = new AttackMove
        {
            moveDeltaWeapon = swordAttack,

            moveDeltaChest = chestMove
        };

    }

    public override void PlayAttack(int index, bool mod = false)
    {
        base.PlayAttack(index, mod);
    }

    public override TransformDelta GetMainTrasnformDeltaInAttackMove()
    {
        return mainAttack.moveDeltaWeapon;
    }

    public override TransformDelta GetChestTrasnformDeltaInAttackMove()
    {
        return mainAttack.moveDeltaChest;
    }

}
