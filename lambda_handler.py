import json
import pymysql
import os

def lambda_handler(event, context):
    # RDSê⁄ë±èÓïÒ
    rds_host = os.environ['RDS_HOST']
    db_name = os.environ['DB_NAME']
    username = os.environ['DB_USERNAME']
    password = os.environ['DB_PASSWORD']
    
    try:
        conn = pymysql.connect(
            host=rds_host,
            user=username,
            passwd=password,
            db=db_name,
            connect_timeout=5
        )
        
        with conn.cursor() as cursor:
            data = json.loads(event['data'])
            
            sql = """INSERT INTO postits 
                     (id, content, goal_count, created_at, updated_at)
                     VALUES (%s, %s, %s, %s, %s)"""
                     
            cursor.execute(sql, (
                data['Id'],
                data['Content'],
                data['GoalCount'],
                data['CreatedAt'],
                data['UpdatedAt']
            ))
            
        conn.commit()
        
        return {
            'statusCode': 200,
            'body': json.dumps('Successfully saved PostIt data')
        }
    
    except Exception as e:
        print(e)
        return {
            'statusCode': 500,
            'body': json.dumps('Database error')
        }