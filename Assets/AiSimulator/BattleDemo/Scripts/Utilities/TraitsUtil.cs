using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IndieDevTools.Traits;

namespace IndieDevTools.Demo.BattleSimulator
{
    /// <summary>
    /// A utility class for getting and setting various traits used in the battle simulator demo.
    /// Note, const strings for trait ids match the trait filenames.
    /// </summary>
    public static class TraitsUtil
    {
        // Move Speed
        const float minSpeed = 0.0f;
        const float maxSpeed = 10.0f;
        const float defaultSpeed = 1.0f;

        const float minMoveSpeed = 0.0f;
        const float maxMoveSpeed = 0.1f;

        public const string speedTraitId = "Speed";

        public static float GetMoveSpeed(IStatsCollection statsCollection)
        {
            return Mathf.Lerp(minMoveSpeed, maxMoveSpeed, GetSpeedPercentage(statsCollection));
        }

        static float GetSpeedPercentage(IStatsCollection statsCollection)
        {
            return Mathf.InverseLerp(minSpeed, maxSpeed, GetSpeed(statsCollection));
        }

        static float GetSpeed(IStatsCollection statsCollection)
        {
            ITrait trait = statsCollection.GetStat(speedTraitId);
            if (trait == null)
            {
                return defaultSpeed;
            }
            return Mathf.Clamp(trait.Quantity * 1.0f, minSpeed, maxSpeed);
        }

        // Move Radius
        public const string moveRadiusTraitId = "MoveRadius";
        public static int GetMoveRadius(IStatsCollection statsCollection)
        {
            ITrait trait = statsCollection.GetStat(moveRadiusTraitId);
            if (trait == null)
            {
                return 0;
            }
            return trait.Quantity;
        }

        // Attack Strength
        const int minAttackStrength = 0;
        const int maxAttackStrength = 10;
        const int defaultAttackStrength = 0;

        public const string attackStrengthTraitId = "Attack";
        public static int GetAttackStrength(IStatsCollection statsCollection)
        {
            ITrait trait = statsCollection.GetStat(attackStrengthTraitId);
            if (trait == null)
            {
                return defaultAttackStrength;
            }
            return Mathf.Clamp(trait.Quantity, minAttackStrength, maxAttackStrength);
        }

        public static int GetRandomAttackStrength(IStatsCollection statsCollection)
        {
            float attackStrength = GetAttackStrength(statsCollection);
            return Mathf.RoundToInt(Random.Range(minAttackStrength, attackStrength));
        }

        public static void SetAttackStrength(IStatsCollection statsCollection, int quantity)
        {
            ITrait trait = statsCollection.GetStat(attackStrengthTraitId);
            if (trait != null)
            {
                trait.Quantity = quantity;
            }
        }

        // Defense Strength
        const int minDefenseStrength = 0;
        const int maxDefenseStrength = 10;
        const int defaultDefenseStrength = 0;

        public const string defenseStrengthTraitId = "Defense";
        public static int GetDefenseStrength(IStatsCollection statsCollection)
        {
            ITrait trait = statsCollection.GetStat(defenseStrengthTraitId);
            if (trait == null)
            {
                return defaultDefenseStrength;
            }
            return Mathf.Clamp(trait.Quantity, minDefenseStrength, maxDefenseStrength);
        }

        public static int GetRandomDefenseStrength(IStatsCollection statsCollection)
        {
            float defenseStrength = GetDefenseStrength(statsCollection);
            return Mathf.RoundToInt(Random.Range(minDefenseStrength, defenseStrength));
        }

        public static void SetDefenseStrength(IStatsCollection statsCollection, int quantity)
        {
            ITrait trait = statsCollection.GetStat(defenseStrengthTraitId);
            if (trait != null)
            {
                trait.Quantity = quantity;
            }
        }

        // Health
        public const string healthTraitId = "Health";
        public static int GetHealth(IStatsCollection statsCollection)
        {
            ITrait trait = statsCollection.GetStat(healthTraitId);
            if (trait == null)
            {
                return 0;
            }
            return trait.Quantity;
        }

        public static void SetHealth(IStatsCollection statsCollection, int quantity)
        {
            ITrait trait = statsCollection.GetStat(healthTraitId);
            if (trait != null)
            {
                trait.Quantity = quantity;
            }
        }

        // Size
        public const string sizeTraitId = "Size";
        public static int GetSize(IStatsCollection statsCollection)
        {
            ITrait trait = statsCollection.GetStat(sizeTraitId);
            if (trait == null)
            {
                return 0;
            }
            return trait.Quantity;
        }

        public static void SetSize(IStatsCollection statsCollection, int quantity)
        {
            ITrait trait = statsCollection.GetStat(sizeTraitId);
            if (trait != null)
            {
                trait.Quantity = quantity;
            }
        }
    }
}
