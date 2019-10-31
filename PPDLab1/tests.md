## Values used:
* 100 accounts
* 1000000 transactions

|                  | 1 Thread | 2 Threads | 4 Threads | 8 Threads | 16 Threads |
|------------------|----------|-----------|-----------|-----------|------------|
| Per-Account Lock | ~33241ms | ~32216ms  | ~33737ms  | ~31857ms  | ~402645ms  |
| Global Lock      | ~39003ms | ~49695ms  | ~43897ms  | ~42771ms  | ~45375ms   |