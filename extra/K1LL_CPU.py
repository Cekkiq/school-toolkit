import random
import time
import math
import numpy as np
from datetime import datetime

def heavy_computation_factorial(n):
    result = []
    for i in range(n):
        num = random.randint(1, 1000)
        factorial = math.factorial(num)
        result.append(factorial)
    return result

def heavy_computation_matrix(n):
    result = []
    for i in range(n):
        matrix = np.random.rand(100, 100)
        inv_matrix = np.linalg.inv(matrix)
        result.append(inv_matrix)
    return result

if __name__ == "__main__":
    num = int(input("Enter the number of heavy computations to run (Reccomended 10,000: "))

    start_time = time.time()
    
    numbers = heavy_computation_matrix(num)
    
    end_time = time.time()

    current_time = datetime.now().strftime("%Y-%m-%d_%H-%M-%S")
    filename = f"K1LL_CPU_{current_time}.txt"
    
    with open(filename, "w") as file:
        file.write(f"Completed {num} heavy computations in {end_time - start_time:.4f} seconds.\n")
        file.write(f"Computation results:\n")
        file.write(str(numbers))
    
    print(f"Completed {num} heavy computations in {end_time - start_time:.4f} seconds. Results saved to {filename}.")
