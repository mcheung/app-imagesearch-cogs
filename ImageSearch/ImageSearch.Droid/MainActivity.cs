using System;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using ImageSearch.Droid.Adapters;
using ImageSearch.ViewModel;

using Plugin.Permissions;
using Acr.UserDialogs;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Content.PM;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace ImageSearch.Droid
{
    [Activity(Label = "Happy Happy Happy", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {
        ImageSearchViewModel viewModel;
        String ImageType = "kittens";
        double Percentage = 100.0;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.main);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            viewModel = new ImageSearchViewModel();

            //var progress = FindViewById<ProgressBar>(Resource.Id.my_progress);
            //var query = FindViewById<EditText>(Resource.Id.my_query);
            //var button = FindViewById<Button>(Resource.Id.my_button);

            //button.Click += async (sender, args) =>
            //{
            //    button.Enabled = false;
            //    progress.Visibility = ViewStates.Visible;

            //    await viewModel.SearchForImagesAsync(string.Format("cute {0}", query.Text.Trim()));

            //    progress.Visibility = ViewStates.Gone;
            //    button.Enabled = true;
            //};

            SetupMainView();
            SetupCamera();
        }

        RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;
        ImageAdapter adapter;

        void SetupMainView()
        {
            adapter = new ImageAdapter(this, viewModel);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

            recyclerView.SetAdapter(adapter);

            layoutManager = new GridLayoutManager(this, 1);

            recyclerView.SetLayoutManager(layoutManager);

            UserDialogs.Init(this);
        }

        void SetupCamera()
        {
            adapter.ItemClick += async (sender, args) =>
            {
                var image = viewModel.Images[args.Position].ThumbnailLink;
                //var percentage = await viewModel.AnalyzeImageAsync(image);
                await viewModel.ShowImageMessage(ImageType, Percentage);
            };

            var fab = FindViewById<FloatingActionButton>(Resource.Id.fab_photo);
            fab.Visibility = ViewStates.Visible;

            fab.Click += async (sender, args) =>
            {
                //Get references to elements
                var progress = FindViewById<ProgressBar>(Resource.Id.my_progress);
                var happinessLevelText = FindViewById<TextView>(Resource.Id.happiness_level_text);
                var helpCardText = FindViewById<TextView>(Resource.Id.help_card_text);
                var helpCard = FindViewById<CardView>(Resource.Id.help_card);

                //Hide startup and results elements
                fab.Enabled = false;
                fab.Visibility = ViewStates.Gone;
                recyclerView.Visibility = ViewStates.Gone;
                happinessLevelText.Visibility = ViewStates.Gone;

                //Show progress elements
                progress.Visibility = ViewStates.Visible;
                helpCardText.Text = "Analysing your happiness...";
                helpCardText.Visibility = ViewStates.Visible;
                helpCard.Visibility = ViewStates.Visible;

                //Go to photo app then analyse over emotion api
                Percentage = await viewModel.TakePhotoAndAnalyzeAsync();
                
                //Change analysing text to results text
                var text = string.Format("You are {0}% happy! \nCute coming right up...", Percentage);
                helpCardText.Text = text;

                //Set the images to get
                SetImageType();

                //Search for the images
                await viewModel.SearchForImagesAsync(string.Format("cute {0}", ImageType.Trim()));

                //Set happiness level text
                happinessLevelText.Text = string.Format("You are {0}% happy as {1}", Percentage, ImageType);

                //Hide progress elements
                progress.Visibility = ViewStates.Gone;
                helpCardText.Visibility = ViewStates.Gone;
                helpCard.Visibility = ViewStates.Gone;

                //Show results and restart elements
                fab.Visibility = ViewStates.Visible;
                happinessLevelText.Visibility = ViewStates.Visible;
                recyclerView.Visibility = ViewStates.Visible;
                fab.Enabled = true;
            };
        }

        private void SetImageType()
        {
            ImageType = "kittens";
            if (Percentage < 90.0)
                ImageType = "puppies";
            if (Percentage < 80.0)
                ImageType = "ducklings";
            if (Percentage < 70.0)
                ImageType = "lambs";
            if (Percentage < 60.0)
                ImageType = "meerkats";
            if (Percentage < 50.0)
                ImageType = "pandas";
            if (Percentage < 40.0)
                ImageType = "piglets";
            if (Percentage < 30.0)
                ImageType = "koalas";
            if (Percentage < 20.0)
                ImageType = "chimpanzees";
            if (Percentage < 10.0)
                ImageType = "owls";
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

