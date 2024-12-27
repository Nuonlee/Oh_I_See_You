import torch
import socket
import pickle

# CUDA 사용 가능 여부 확인
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")

# 서버 소켓 설정
HOST = '127.0.0.1'  # 로컬 호스트
PORT = 65432  # 임의의 포트 번호

# 소켓 생성
with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((HOST, PORT))
    s.listen()
    print("Python 서버가 시작되었습니다. Unity에서 연결을 기다리는 중...")

    conn, addr = s.accept()
    with conn:
        print(f"{addr}와 연결되었습니다.")

        # PyTorch 텐서 연산
        tensor_a = torch.randn(3, 3, device=device)
        tensor_b = torch.randn(3, 3, device=device)
        tensor_sum = tensor_a + tensor_b

        # 텐서 결과를 바이트 형태로 직렬화 (pickle 사용)
        data = pickle.dumps(tensor_sum.cpu().numpy())  # GPU에서 CPU로 이동 후 전송
        conn.sendall(data)
        print("연산 결과를 전송했습니다.")
