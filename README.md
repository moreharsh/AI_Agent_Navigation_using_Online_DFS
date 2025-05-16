# AI_Agent_Navigation_using_Online_DFS
 This project implements an AI agent in Unity that navigates a maze-like, unknown environment using a modified Depth-First Search (DFS) algorithm, adapted to follow an online search strategy.

# Task Environment

### The agent operates in an environment with the following characteristics:

- Partially Observable: The agent does not have full knowledge of the environment's layout and can only perceive its immediate surroundings through raycasts (like a robot with local sensors).

- Continuous: The agent navigates in continuous 3D space rather than a discrete grid.

- Sequential: Each decision affects the future, making the sequence of actions critical.

- Unknown: The environment is not known beforehand; no map or layout is pre-loaded.

- Static: The environment does not change while the agent is exploring.

- Single Agent: Only one autonomous agent is involved in the decision-making process.

- Non-Deterministic: Due to potential variability in physics or movement precision, the result of an action (e.g., turning a corner) may not always be perfectly predictable.

# Why Online Search?

In classical search algorithms like DFS or A*, a complete map of the environment is typically known in advance. However, in real-world robotic or simulated agents, the agent must explore the environment while planning, without access to a full map. This leads to the use of online search algorithms, where decisions are made based on local observations, and the agent gradually builds knowledge of the environment through exploration.

# Online search is particularly useful in robotics, exploration AI, and game AI, where the environment may be:

- too large to store completely,

- dynamically revealed,

- or partially hidden due to fog-of-war or line-of-sight limitations.

# Modified DFS for Online Navigation

This project modifies the traditional Depth-First Search (DFS) to suit an online execution model, where the agent does not have access to a full environment graph:

- The agent explores step-by-step, using raycasts to detect walls and open paths (left, right, and forward).

- At decision points (where multiple paths are available), it pushes the position and orientation onto a stack.

- When it encounters a dead-end, it switches to backtracking mode, popping from the stack and returning to an earlier decision point.

- As it revisits unexplored paths, the process continues, allowing the agent to eventually explore all reachable areas.

# Key Features

- Online Search Logic: Decision-making is done incrementally based on real-time environment feedback.

- Raycast-Based Obstacle Detection: Uses forward, left, and right raycasts to detect walls.

- Stack-Based DFS Traversal: Mimics recursive DFS using a position-angle stack for backtracking.

- Backtracking Mode: Navigates back to the last unexplored junction when trapped.

- Centering Mechanism: Keeps the agent centered between walls for smooth motion.

- Implemented from Scratch: Entire navigation logic written manually using Unity’s Transform, NavMeshAgent, and Physics APIs—no third-party navigation systems.

# How It Works

- Exploring Phase:

* The agent moves forward until it hits an obstacle.

* If a left or right opening is detected, the mid-point is stored on a stack along with the turning angle.

* When no forward path exists, the agent rotates to explore open sides.

- Backtracking Phase:

* If all directions are blocked, the agent pops a position from the stack and returns to that location.

* On reaching the stored point, it rotates to the stored direction and resumes exploration.

- Continuous Adjustment:

* The agent constantly checks distances to the left and right walls and re-centers itself between them using vector interpolation.

# Video Demo
