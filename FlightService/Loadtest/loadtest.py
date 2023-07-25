import concurrent.futures
import requests

url = "http://k8s.hochedlinger.at/api/flights"
num_workers = 20

def send_request(url):
    while True:
        try:
            response = requests.get(url)
        except requests.exceptions.RequestException as e:
            pass

with concurrent.futures.ThreadPoolExecutor(max_workers=num_workers) as executor:
    futures = [executor.submit(send_request, url) for _ in range(num_workers)]
    for future in concurrent.futures.as_completed(futures):
        future.result()
