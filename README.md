## **Game Title**

We feel like we need a chance to play and polish the game before settling on a final name for it. But a potential title could be “Clashout”.

## **Overview**

Clashout (name not final) is a 3D multiplayer arena battle  game where 2 players compete to obtain the most stars, by collecting them as they spawn around the map or stealing them from other players.

## **Core Gameplay**

Our game is a multiplayer arena battle where you must run around a large 3D map that spawns “stars” (subject to change) at a fixed interval in random set locations. The goal of the game is to collect more stars than your opponent, while avoiding their attacks which allow them to steal stars from you.

Each player (alongside their item) has a default short-ranged melee attack that knocks back the opponent and causes them to lose a star after enough successive hits. If we have time, we would also like to add a stronger secondary attack for added strategy. When an opponent loses a star, they gain temporary invincibility as well as a speed boost, but cannot attack during this period.

Alongside the other player, AI enemies can also appear at set intervals in the arena utilizing special items. If you are hit by one of the enemy’s attacks, you lose a star. If you can defeat the enemy, you can temporarily utilize the item they were holding.

We will add as many unique items as we can to the game, ranging from powerful weapons like grenades to temporary movement-enhancing abilities like a grappling hook. Items will be balanced with pros and cons that encourage players to experiment with various playstyles.

When one player reaches a certain number of stars (10, possibly customizable) or the time limit is reached, the game will end and a winner will be chosen.

If we have time, we will try to add multiple maps to the game, as well as lots of customizable rules for the battles (such as how often stars spawn, or even how AI enemies should behave)

**Example:** Player 1 and 2 spawn in the middle of the arena. A star spawns in the far north, visible to both players. Player 1 rushes to the star, outrunning Player 2\. In the meantime, Player 2 travels somewhere else to fight an AI enemy, dropping a grappling hook with limited durability. Player 2 catches up to Player 1 and successfully grapples them, pulling them away and causing them to drop a star. A short while later, another star spawns and the battle continues.

## **Game Type**  
Our game is a mixture between an Arena Battle, Maze Chase, and First Person Shooter. Both players are competing with one another to collect stars, utilizing melee combat and items to steal from one another.

## **Player Setup**

There will be two human-controlled players. We will initially do this locally (one on mouse and keyboard, the other on controller), but attempt to move to online multiplayer if we are able to make it work.

## **AI Enemies**  
Our enemies will spawn in designated spots where items can be collected. They will move around, but won’t stray too far from their spawn point. The AI enemies will be hostile towards the player(s), and will use their item to attack them. Defeating the AI enemies will be easier than defeating the human opponent and will reward  the player with the enemy’s item.

**AI FSM:**

* Idle  (Wait for a player to walk into its path)
* Launch/Attack (use specific items to launch towards/attack player, etc)  
* Target (target and pathfind towards nearby player)

## **Scripted Events**  
As the game goes on, stars and AI enemies will randomly spawn across the map at fixed intervals. Players can (should) make their way towards these to increase their chance of winning or earning stars.

## **Environment**  
The game will take place in a fairly open grassy arena with various platforms, obstacles, hills, and more. The terrain will be fairly simple, keeping it easy for an AI agent to pathfind and navigate effectively. We will either do this with Unity’s terrain feature, or simply by using lots of cubes with different textures on them. Extra 3D models like grass, trees, and more can be added for detail.

## **Assets**  
Most sounds, models, and other assets will likely be taken from free packs on the Unity Asset Store, itch.io, and more.

Various free websites and tools may also be of use, such as paint.net for graphics, Audacity for audio editing, and Mixamo for 3D animations.

