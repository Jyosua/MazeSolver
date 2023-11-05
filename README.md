# MazeSolver
This was originally part of a technical assignment I solved for Bluebeam nearly a decade ago. Their hiring challenges have changed since then, so I'm making this repository public now and also updating it to more modern C# and using many of the practices I've learned over the years.

## Original Requirements
### Description
You are asked to write a command line program in C# that takes a maze image as input and solves the maze and writes it to an output image. Please see the attached sample maze inputs. Note that the samples do not represent all types of mazes that your solution if expected to solve. The syntax of the program must be:

`maze.exe "source.[bmp,png,jpg]" "destination.[bmp,png,jpg]"`

### Rules:

1.	Start at Red
2.	Finish at Blue
3.	All walls are black
4.	Maze is completely surrounded by black walls
5.	Draw the solution on the image in green

### Grading Considerations
Specifically, there are three things we will be looking for:
- Well-organized original code that demonstrates your ability to turn a complicated problem into an efficient and maintainable solution
- Efficient solution that solves the problem fast without using too much memory.
- A dynamic solution that can solve mazes of many forms (not all orthogonal) that meet the basic requirements (Red is start, Blue is finish, and Black for walls)
- (Bonus points for unit testing)

## Notes
### Retrospective
There were definitely some ambiguities when I received the requirements, so I sent an e-mail back to clarify things like tolerances, preferred file type handling, etc. I was interviewing for an entry-level position at the time, so I was a much, much greener developer. I ended up not having enough time to add unit tests, the code I wrote was very, very functional and procedural in style, and, looking back at it now, what I wrote was pretty hard to read -- especially in certain places.

### Implementation
I implemented the A* pathfinding algorithm, because I initially thought Dijkstra's algorithm would be the correct choice and after researching that, found out that adding a heuristic makes it more efficient. In order to implement this algorithm, you need a datastructure that you can add elements to and keep them sorted as you add in reasonable execution time, but you also need to be able to retrieve the lowest-value item in the structure in constant or near-constant time. You also need to be able to update the priorities for neighboring nodes sometimes.

This is actually describing a priority queue, but C# at the time did not have that data structure -- you had to implement it yourself or use someone else's. I preferred to stick to language APIs whenever possible, so after some thought, realized that I could use a SortedSet with the constraint that it would not properly favor nodes with the same priority in relation to the order they were added. Instead, this solution would favor the top-leftmost nodes, but it was still pretty good considering the problem.