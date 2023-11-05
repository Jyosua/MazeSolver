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

Upon refactoring this now in 2023, C# has recently added a PriorityQueue class, but it has a serious limitation -- you cannot update the priority of items that already exist in the queue and you can't remove items except by calling Dequeue. This problem rules out being able to use it for A*, because you would effectively need to dequeue until you get to the node you need and Re-enqueue everything you dequeued up until that point to make it work. Since I went through the effort of refactoring this, I wanted to implement it correctly, so this time I've changed the structure representing the open node list to use [FastPriorityQueue](https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp).

Also, because this was originally written for Windows, it unfortunately utilizes the System.Drawing.Bitmap class. I think I would like to remove this and make the application truly cross platform in a later revision, but this refactor was already quite a lot of work as it is.

### General Practices
In refactoring this, I tried to prioritize the following things:

- Code readability
- Keeping code complexity low
- Proper separation of concerns
- Limiting mutability only to where it was really, really needed for performance or due to how API needed to be structured
- Minimizing allocations where possible, as these can be expensive in C#
- Favoring computational performance over the amount of memory used
- Adding some unit tests

In the end, I haven't added as many unit tests as I would like. I really only was able to finish unit testing the arguments and file checking logic, not the Graph construction or PathFinding yet. As I add more unit tests, the number of interfaces and the number of layers of abstraction will increase, though, so there's some tradeoffs if considering complexity.