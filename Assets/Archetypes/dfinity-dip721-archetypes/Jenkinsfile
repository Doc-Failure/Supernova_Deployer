/* def gv */

pipeline {
    agent {
        docker { image 'node:14.18.2' }
    }
    parameters {
        //choice(name: 'VERSION', choices: ['1.1.0', '1.2.0', '1.3.0'], description: '')
        //booleanParam(name: 'executeTests', defaultValue: true, description: '')
        text(name: 'NAME', defaultValue: 'test', description: 'Enter the name of the contract')
        text(name: 'TOKENS_LIST', defaultValue: '', description: 'Enter the tokens separated by ,')
        text(name: 'IPFS_ENDPOINT', defaultValue: '', description: 'Enter address of the metadata resources')
        text(name: 'MINTING_PROBABILITY', defaultValue: '0', description: 'Enter Probability to mint a token')
        text(name: 'MINTING_QUANTITY', defaultValue: '0', description: 'Enter the quantity to mint')
        text(name: 'CHAIN_ID', defaultValue: '0', description: 'Enter the Chain ID')
    }
    stages {
        stage("init") {
            steps {
                echo 'init'
                sh 'export INFURA_API_KEY=f8abcb045ed64fb0a51b1e67ec54888d'
                sh 'npm install' 
                sh "npm run generate \"${params.NAME}\" \"${params.TOKENS_LIST}\" \"${params.IPFS_ENDPOINT}\" \"${params.MINTING_PROBABILITY}\" \"${params.MINTING_QUANTITY}\"" 
            }
        }
        stage("build") {
            steps {
                echo 'building'
                sh 'npm run build' 
            }
        }
        stage("test") {
            steps {
                echo 'test'
            } 
        }
        stage("deploy") {
            steps {
                echo 'deploy'
                 script {
                    if (params.CHAIN_ID=="97"){
                        sh "npm run bsc_testnet:deploy"
                    }
                    if (params.CHAIN_ID=="4"){
                            sh "npm run rinkeby:deploy"
                    }

                    if (params.CHAIN_ID=="80001"){
                            sh "npm run mumbai:deploy"
                    }
                    sh "curl -X 'POST' \
                            'https://api.nft.storage/upload' \
                            -H 'accept: application/json' \
                            -H 'Content-Type: */*' \
                            -H 'Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkaWQ6ZXRocjoweEI5RTEyMGFhRjQyMWNGNzJiNzAxOEU3ZUFlRDljNWYwOTBERDYxOGQiLCJpc3MiOiJuZnQtc3RvcmFnZSIsImlhdCI6MTY1Mjk3MDQxNDgwMCwibmFtZSI6IlVuaXR5V2ViMyJ9.8gBlTY7DiGTi2yulLTI6qawpdnQLtqDRJFLgsohMrt0' \
                            --data-binary '@deployments/bsc_testnet/NFTContract.json'"
                 }
            }
        }
    }   
}

