using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using System.Text.Json;
using System.Windows;

public class AwsStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _lambdaFunctionName;

    public AwsStorageService()
    {
        _s3Client = new AmazonS3Client();
        _bucketName = "your-bucket-name";
        _lambdaFunctionName = "your-lambda-function-name";
    }

    public async Task SaveToS3(PostItData postIt)
    {
        try
        {
            var json = JsonSerializer.Serialize(postIt);
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = $"postits/{postIt.Id}.json",
                ContentBody = json
            };

            await _s3Client.PutObjectAsync(putRequest);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"S3への保存に失敗しました: {ex.Message}");
        }
    }
}