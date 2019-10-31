## Values used:
* Both matrices were 1000x1000
* Timers were started before thread creation and ended after all threads finished

|          | 1 Thread | 2 Threads | 4 Threads | 8 Threads | 16 Threads | 32 Threads |
|----------|----------|-----------|-----------|-----------|------------|------------|
| Addition | ~56ms    | ~46ms     | ~86ms     | ~179ms    | ~279ms     | ~587ms     |
| Product  | ~38753ms | ~20876ms  | ~13384ms  | ~11218ms  | ~11865ms   | ~12248ms   |