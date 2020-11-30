from flask import Flask
from flask import request
app = Flask(__name__)

@app.route('/api', methods=['POST'])
def hello():
    print(request.data)
    return "OK"

if __name__ == '__main__':
    app.run(host='0.0.0.0' , port=5000)