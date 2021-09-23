using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace DropParty
{
    public interface IDummyCoin
    {
        /// <summary>
        /// The position of this coin
        /// </summary>
        Vector2f Position { get; }
        /// <summary>
        /// The value of this coin. It will be 1, 2, or 3
        /// </summary>
        int Value { get; }
        /// <summary>
        /// True if the coin is being held by a player right now.
        /// </summary>
        bool isHeld { get; }
        /// <summary>
        /// The radius of the coin
        /// </summary>
        float Radius { get; }
        IDummyCoin DeepCopy();
    }

    public interface IDummyPlayer
    {
        /// <summary>
        /// The position of the player.
        /// </summary>
        Vector2f Position { get; }
        /// <summary>
        /// The player's name
        /// </summary>
        string PlayerName { get; }
        /// <summary>
        /// The radius of the player
        /// </summary>
        float Radius { get; }
        /// <summary>
        /// The value of all coins currently held by this player
        /// </summary>
        int HeldValue { get; }
        /// <summary>
        /// The value of all coins in this player's bank
        /// </summary>
        int BankedValue { get; }
        IDummyPlayer DeepCopy();
    }

    public interface IDummyBase
    {
        /// <summary>
        /// The position of this base
        /// </summary>
        Vector2f Position { get; }
        /// <summary>
        /// The name of the player this base belongs to
        /// </summary>
        string PlayerName { get; }
        /// <summary>
        /// The radius of this base
        /// </summary>
        float Radius { get; }
        IDummyBase DeepCopy();
    }

    public interface IDummyBullet
    {
        /// <summary>
        /// The position of the bullet
        /// </summary>
        Vector2f Position { get; }
        /// <summary>
        /// The velocity of this bullet. This is in units per frame
        /// </summary>
        Vector2f Velocity { get; }
        /// <summary>
        /// The radius of the bullet
        /// </summary>
        float Radius { get; }
        IDummyBullet DeepCopy();
    }
}
