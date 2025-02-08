import torch
import torch.nn as nn
import torch.optim as optim
import numpy as np
import GridCreation as gc

class BlockHintNet(nn.Module):
    def __init__(self):
        super(BlockHintNet, self).__init__()
        
        self.fc1 = nn.Linear()
        