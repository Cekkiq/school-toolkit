import torch
import random
import time
import numpy as np
import psutil
import argparse
from datetime import datetime
import GPUtil

parser = argparse.ArgumentParser()
parser.add_argument("--nosafe", action="store_true", help="Disable GPU safety check")
args = parser.parse_args()

def check_gpu_usage(threshold=90):
    gpus = GPUtil.getGPUs()
    return any(gpu.load * 100 > threshold for gpu in gpus)

def heavy_computation_gpu(n, device):
    result = []
    for _ in range(n):
        tensor = torch.randn(1000, 1000, device=device)
        # Perform a large matrix multiplication which is GPU-intensive
        result.append(torch.matmul(tensor, tensor))  
        if not args.nosafe and check_gpu_usage():
            print("GPU load too high, exiting...")
            return result
    return result

def check_cpu_usage(threshold=90):
    return psutil.cpu_percent(interval=1) > threshold

device = torch.device("cuda" if torch.cuda.is_available() else "cpu")

if __name__ == "__main__":
    num = int(input("Enter the number of heavy computations to run (Recommended 10,000): "))

    start_time = time.time()
    
    numbers = heavy_computation_gpu(num, device)  # GPU-heavy operation
    
    end_time = time.time()

    current_time = datetime.now().strftime("%Y-%m-%d_%H-%M-%S")
    filename = f"K1LL_GPU_{current_time}.txt"
    
    with open(filename, "w") as file:
        file.write(f"Completed {len(numbers)} heavy computations in {end_time - start_time:.4f} seconds.\n")
        file.write(f"Computation results:\n")
        file.write(str(numbers))
    
    print(f"Completed {len(numbers)} heavy computations in {end_time - start_time:.4f} seconds. Results saved to {filename}.")
